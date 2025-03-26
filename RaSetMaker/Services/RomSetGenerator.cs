using RaSetMaker.Models;
using RaSetMaker.Persistence;
using RaSetMaker.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RaSetMaker.Services
{
    public class RomSetGeneratorProgress
    {
        public double totalProgress = 0;
        public double systemProgress = 0;
        public string currentSystem = "";
        public string currentFile = "";
        public List<string> exceptions = [];
    }

    public class RomSetGeneratorResult
    {
        public int AddedRoms = 0;
        public int RemovedRoms = 0;
    }

    public class RomSetGenerator(Ra2DatContext context)
    {
        public async Task<RomSetGeneratorResult> GenerateSet(IProgress<RomSetGeneratorProgress> progress, CancellationToken cancellationToken)
        {
            RomSetGeneratorResult result = new();

            await Task.Run(() =>
            {
                result.RemovedRoms = PrepareDirs(progress);
            });

            result.AddedRoms = await MatchToRom(progress, cancellationToken);

            return result;
        }

        private int PrepareDirs(IProgress<RomSetGeneratorProgress> progress)
        {
            RomSetGeneratorProgress progressInfo = new()
            {
                currentSystem = "Preparing directories..."
            };

            progress.Report(progressInfo);

            var inDirInfo = new DirectoryInfo(context.UserConfig.InputRomsDirectory);
            var outDirInfo = new DirectoryInfo(context.UserConfig.OutputRomsDirectory);
            var dirStyle = context.UserConfig.DirStructureStyle;

            var expectedDirs = context.GetSystems().Select(s =>
            {
                var dirName = s.GetDirName(dirStyle);

                if (string.IsNullOrEmpty(dirName))
                {
                    throw new Exception($"Invalid rom set dir name for {s.Name} and style {dirStyle}");
                }

                return dirName;
            }).ToImmutableHashSet();

            inDirInfo.Create();
            outDirInfo.Create();

            // Iterate output dir subdirs and move all unexpected subdirs to input
            foreach (var outSubDir in outDirInfo.GetDirectories())
            {
                if (expectedDirs.Contains(outSubDir.Name) == false)
                {
                    outSubDir.MoveTo(DirUtils.UniquePath($"{inDirInfo.FullName}/{outSubDir.Name}"));
                }
            }

            var filesInDb = context
                .GetSystems()
                .SelectMany(s => s.Games)
                .SelectMany(g => g.Roms)
                .SelectMany(r => r.RomFiles)
                .Select(rf => new FileInfo(Path.Combine(outDirInfo.FullName, rf.FilePath)).FullName)
                .ToImmutableHashSet();

            // Iterate output dir recursively and move all files not linked to Roms to input
            int removedRoms = 0;
            foreach (var file in outDirInfo.GetAllFilesRecursive().Select(f => new FileInfo(f)))
            {
                if (!filesInDb.Contains(file.FullName))
                {
                    var parentDir = new DirectoryInfo($"{inDirInfo.FullName}{file.Directory?.Name ?? ""}");
                    parentDir.Create();

                    var destPath = $"{parentDir.FullName}/{file.Name}";
                    if (!Path.Exists(destPath))
                    {
                        file.MoveTo($"{parentDir.FullName}/{file.Name}");
                        ++removedRoms;
                    }
                }
            }

            // Iterate all expected dirs and create missing ones
            foreach (var expDir in expectedDirs)
            {
                outDirInfo.CreateSubdirectory(expDir);
            }

            return removedRoms;
        }

        private async Task<int> MatchToRom(IProgress<RomSetGeneratorProgress> progress, CancellationToken cancellationToken)
        {
            RomSetGeneratorProgress progressInfo = new();

            var inDirInfo = new DirectoryInfo(context.UserConfig.InputRomsDirectory);
            var outDirInfo = new DirectoryInfo(context.UserConfig.OutputRomsDirectory);
            var dirStyle = context.UserConfig.DirStructureStyle;

            double totalProgressStep = 100.0 / context.GetSystems().Count();

            var inFiles = inDirInfo.GetAllFilesRecursive().ToHashSet();

            var addedRoms = 0;
            foreach (var system in context.GetCheckedSystems().Where(s => s.GetGamesMatchingFilter().Any()))
            {
                var gameSystemDir = system.GetDirName(dirStyle);

                var matcher = system.CreateMatcher();

                HashSet<string> movedFiles = [];

                double systemProgressStep = 100.0 / (double)inFiles.Count;

                progressInfo.currentSystem = $"Processing {system.Name}";
                progressInfo.systemProgress = 0;

                foreach (var file in inFiles)
                {
                    var fileInfo = new FileInfo(file);
                    if (!fileInfo.Exists)
                    {
                        continue;
                    }

                    if (cancellationToken.IsCancellationRequested)
                    {
                        progressInfo.currentSystem = "Operation cancelled by user";
                        progressInfo.currentFile = "";
                        progressInfo.totalProgress = 0;
                        progressInfo.systemProgress = 0;
                        progress.Report(progressInfo);
                        return addedRoms;
                    }

                    progressInfo.currentFile = file;
                    progress.Report(progressInfo);

                    try
                    {
                        var (rom, romFiles) = matcher.FindRom(fileInfo);


                        // If rom was found, move it to proper outut subdir and update rom file path
                        if (rom != null)
                        {
                            // If rom has multiple files, create subdir for them
                            var romSubdir = romFiles.Count > 1 ? Path.GetFileNameWithoutExtension(romFiles[0]) : "";

                            foreach (var romFile in romFiles.Select(f => new FileInfo(f)))
                            {
                                var outRelPath = $"{gameSystemDir}/{romSubdir}/{romFile.Name}";
                                var outAbsPath = $"{outDirInfo.FullName}{outRelPath}";

                                var outFileInfo = new FileInfo(outAbsPath);

                                outFileInfo.Directory?.Create();

                                romFile.MoveTo(outAbsPath, true);
                                movedFiles.Add(file);
                                rom.RomFiles.Add(new()
                                {
                                    FilePath = outRelPath,
                                    Crc32 = await ComputeCrc32(outAbsPath),
                                });
                            }

                            addedRoms++;
                        }
                    }
                    catch (Exception ex)
                    {
                        progressInfo.exceptions.Add(ex.Message);
                    }


                    progressInfo.systemProgress += systemProgressStep;
                }

                inFiles = [.. inFiles.Except(movedFiles)];

                progressInfo.totalProgress += totalProgressStep;
            }

            inDirInfo.CleanEmptySubdirs();

            progressInfo.totalProgress = 100;
            progressInfo.systemProgress = 100;
            progressInfo.currentSystem = "Done";
            progressInfo.currentFile = "";
            progress.Report(progressInfo);

            return addedRoms;
        }

        private static async Task<uint> ComputeCrc32(string romPath)
        {
            if (Path.GetExtension(romPath) == ".zip")
            {
                using var archive = ZipFile.OpenRead(romPath);
                return archive.Entries.First().Crc32;
            }

            using var file = File.OpenRead(romPath);
            return await Patcher.ComputeChecksum(file);
        }
    }
}
