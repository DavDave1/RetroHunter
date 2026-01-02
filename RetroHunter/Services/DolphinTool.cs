using Microsoft.Extensions.Logging;
using RetroHunter.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RetroHunter.Services
{
    public class DolphinTool(ILogger<DolphinTool> logger, string exePath) : ICompressService
    {


        public async Task CompressRom(UserConfig config, Rom rom, IProgress<CompressProgress>? progress = null)
        {
            if (!Path.Exists(exePath))
                throw new Exception($"DolphinTool exe not found at {exePath}");

            var inputFile = rom.RomFiles.FirstOrDefault();

            if (inputFile == null)
                throw new ArgumentException($"ROM {rom.RaName} is missing file");


            var inputFileInfo = new FileInfo(inputFile.AbsolutePath());
            var outFilename = inputFileInfo.Name.Replace(inputFileInfo.Extension, ".rvz");
            var outAbsPath = inputFile.AbsolutePath().Replace(inputFileInfo.Extension, ".rvz");

            await Compress(inputFile.AbsolutePath(), outAbsPath, progress);

            rom.RomFiles.ForEach(file => File.Delete(Path.Combine(config.OutputRomsDirectory, file.FilePath)));

            rom.AddRomFile(outFilename, await RomPatcher.Patcher.GetSourceCrc32(outAbsPath));

        }

        public async Task Compress(string inputPath, string outputPath, IProgress<CompressProgress>? progress = null)
        {
            progress?.Report(new CompressProgress { Percent = 0 });
            using var dolphinTool = CreateProcess(inputPath, outputPath);

            if (!dolphinTool.Start())
                throw new Exception("Failed to start chdman process");

            await dolphinTool.WaitForExitAsync();

            progress?.Report(new CompressProgress { Percent = 100 });
            
            if (dolphinTool.ExitCode != 0)
                throw new Exception($"DolphinTool ended with error {dolphinTool.ExitCode}");
        }

        private Process CreateProcess(string inputPath, string outputPath)
        {
            var args = string.Join(" ", ["convert", "-i", $"\"{inputPath}\"", "-o", $"\"{outputPath}\"", "-f", "rvz", "-c", "zstd", "-l", "19", "-b", "131072"]);

            logger.LogInformation("Launching with args: {args}", args);

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                },
                EnableRaisingEvents = true
            };


            return process;
        }
    }
}
