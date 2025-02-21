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
                result.AddedRoms = MatchToRom(progress, cancellationToken);
            });

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

            // Iterate input dir and remove all empty subdirs
            foreach (var inSubdir in inDirInfo.GetDirectories())
            {
                if (inSubdir.IsEmptyRecursive())
                {
                    inSubdir.Delete(true);
                }
            }

            var filesInDb = context
                .GetSystems()
                .SelectMany(s => s.GetGamesMatchingFilter(context.UserConfig.GameTypesFilter))
                .SelectMany(g => g.Roms)
                .Where(r => r.IsValid)
                .Select(r => Path.Combine(outDirInfo.FullName, r.FilePath))
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

        public int MatchToRom(IProgress<RomSetGeneratorProgress> progress, CancellationToken cancellationToken)
        {
            RomSetGeneratorProgress progressInfo = new();

            var inDirInfo = new DirectoryInfo(context.UserConfig.InputRomsDirectory);
            var outDirInfo = new DirectoryInfo(context.UserConfig.OutputRomsDirectory);
            var dirStyle = context.UserConfig.DirStructureStyle;

            double totalProgressStep = 100.0 / (double)context.GetSystems().Count();

            var inFiles = inDirInfo.GetAllFilesRecursive().Select(f => new FileInfo(f)).ToHashSet();

            var addedRoms = 0;
            foreach (var system in context.GetCheckedSystems().Where(s => s.GetGamesMatchingFilter(context.UserConfig.GameTypesFilter).Any()))
            {
                var gameSystemDir = system.GetDirName(dirStyle);

                var matcher = system.CreateMatcher();

                HashSet<FileInfo> movedFiles = [];

                double systemProgressStep = 100.0 / (double)inFiles.Count;

                progressInfo.currentSystem = system.Name;
                progressInfo.systemProgress = 0;

                foreach (var file in inFiles)
                {
                    if (!file.Exists)
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

                    progressInfo.currentFile = file.FullName;
                    progress.Report(progressInfo);

                    var (rom, romFiles) = matcher.FindRom(file);


                    // If rom was found, move it to proper outut subdir and update rom file path
                    if (rom != null)
                    {
                        // If rom has multiple files, create subdir for them
                        var romSubdir = romFiles.Count > 1 ? Path.GetFileNameWithoutExtension(romFiles[0]) : "";

                        // Rom path points to either single file or subdir if multiple
                        // Path is relative to rom set output dir
                        var romFilepath = romFiles.Count > 1 ?
                            $"{gameSystemDir}/{romSubdir}" :
                            $"{gameSystemDir}/{Path.GetFileName(romFiles[0])}";

                        foreach (var romFile in romFiles.Select(f => new FileInfo(f)))
                        {
                            var outFilePath = $"{outDirInfo.FullName}{gameSystemDir}/{romSubdir}/{romFile.Name}";

                            romFile.MoveTo(outFilePath, true);
                            movedFiles.Add(file);
                        }

                        rom.FilePath = romFilepath;
                        addedRoms++;
                    }

                    progressInfo.systemProgress += systemProgressStep;
                }

                inFiles = [.. inFiles.Except(movedFiles)];

                progressInfo.totalProgress += totalProgressStep;
            }

            progressInfo.totalProgress = 100;
            progressInfo.systemProgress = 100;
            progressInfo.currentSystem = "Done";
            progress.Report(progressInfo);

            return addedRoms;
        }
    }
}
