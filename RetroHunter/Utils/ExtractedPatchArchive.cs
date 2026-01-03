
using Microsoft.Extensions.Logging.Abstractions;
using RetroHunter.Models;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RetroHunter.Utils
{
    internal class ExtractedPatchArchive(string archiveFilePath) : IDisposable
    {
        public async Task ExtractPatch()
        {
            var extractDir = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(archiveFilePath));
            _extractDirInfo = Directory.CreateDirectory(extractDir);

            // Open file as archive
            {
                using Stream stream = File.OpenRead(archiveFilePath);
                using var reader = ReaderFactory.Open(stream);

                await Task.Run(() => 
                    reader.WriteAllToDirectory(extractDir, new ExtractionOptions()
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    }));
            }
        }

        public IEnumerable<FileInfo> FindPatchByName(string romName, string extension)
        {
            return _extractDirInfo?
               .EnumerateFiles($"{Path.GetFileNameWithoutExtension(romName)}.{extension}") ?? [];
        }

        public IEnumerable<FileInfo> FindPatchByExtension(string extension)
        {
            return _extractDirInfo?.EnumerateFiles($"*.{extension}") ?? [];
        }

        public void Dispose()
        {
            _extractDirInfo?.Delete(true);
            File.Delete(archiveFilePath);
        }

        private DirectoryInfo? _extractDirInfo;
    }
}
