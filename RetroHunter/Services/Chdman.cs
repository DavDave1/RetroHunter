using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RetroHunter.Models;

namespace RetroHunter.Services;

public class ChdmanProgress
{
    public float Percent { get; set; }
}

public class Chdman(ILogger<Chdman> logger)
{
    public enum CompressType
    {
        CD = 0,
        DVD
    }

    public void SetChdManPath(string path)
    {
        _exePath = path;
    }

    public async Task<bool> CompressRom(UserConfig config, GameSystem system, Rom rom, IProgress<ChdmanProgress>? progress = null)
    {
        var inputFile = rom.RomFiles.FirstOrDefault(rf => Path.GetExtension(rf.FilePath) == ".cue");
        CompressType compressType = CompressType.CD;
        var ext = ".cue";

        if (inputFile == null)
        {
            inputFile = rom.RomFiles.FirstOrDefault(rf => Path.GetExtension(rf.FilePath) == ".iso");
            ext = ".iso";
            compressType = CompressType.DVD;
        }

        if (inputFile == null)
        {
            return false;
        }

        var outFilename = new FileInfo(inputFile.AbsolutePath()).Name.Replace(ext, ".chd");
        var outAbsPath = inputFile.AbsolutePath().Replace(ext, ".chd");

        bool ok = await Compress(compressType, inputFile.AbsolutePath(), outAbsPath, progress);

        if (!ok)
        {
            return false;
        }

        rom.RomFiles.ForEach(file => File.Delete(Path.Combine(config.OutputRomsDirectory, file.FilePath)));

        rom.AddRomFile(outFilename, await RomPatcher.Patcher.GetSourceCrc32(outAbsPath));

        return true;

    }

    public async Task<bool> Compress(CompressType compressType, string inputPath, string outputPath, IProgress<ChdmanProgress>? progress = null)
    {
        using var chdman = CreateProcess(compressType, inputPath, outputPath);

        chdman.ErrorDataReceived += (sender, e) =>
        {
            float comprPerc = 0;
            if (e.Data != null)
            {
                if (e.Data.StartsWith("Compressing,"))
                {
                    _ = float.TryParse(e.Data.AsSpan(13, 4), out comprPerc);
                }
                else if (e.Data.StartsWith("Compression complete"))
                {
                    comprPerc = 100;
                }
            }
            else
            {
                comprPerc = 100;
            }

            logger.LogInformation("chdman out: {data}", e.Data);

            if (progress != null)
            {
                progress.Report(new ChdmanProgress { Percent = comprPerc });
            }
        };

        if (!chdman.Start())
        {
            return false;
        }

        chdman.BeginErrorReadLine();

        await chdman.WaitForExitAsync();

        return chdman.ExitCode == 0;
    }

    public bool Detect()
    {
        if (Path.Exists(_exePath))
        {
            return true;
        }

        string? pathEnv = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrEmpty(pathEnv))
        {
            logger.LogError("PATH environment variable not found.");
            return false;
        }


        string[] paths = pathEnv.Split(Path.PathSeparator);

        string executableName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "chdman.exe" : "chdman";

        foreach (string path in paths)
        {
            string fullPath = Path.Combine(path, executableName);

            if (File.Exists(fullPath))
            {
                _exePath = fullPath;
                logger.LogInformation($"chdman exe found at {fullPath}");
                return true;
            }
        }

        return false;
    }

    private Process CreateProcess(CompressType compressType, string inputPath, string outputPath)
    {
        var args = string.Join(" ",
            [compressType == CompressType.CD ? "createcd" : "createdvd", "-i", $"\"{inputPath}\"", "-o", $"\"{outputPath}\"", "-f"]);

        logger.LogInformation("Launching with args: {args}", args);

        var chdman = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _exePath,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            },
            EnableRaisingEvents = true
        };


        return chdman;
    }

    private string _exePath = "";
}