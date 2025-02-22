using RaSetMaker.Models;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace RaSetMaker.Utils.Matchers
{
    public abstract class MatcherBase(GameSystem system)
    {
        public abstract (Rom?, List<string>) FindRom(FileInfo file);

        protected Rom? MatchRomByHash(string hash)
        {
            foreach (var game in system.Games)
            {
                foreach (var rom in game.Roms)
                {
                    if (rom.Hash == hash)
                    {
                        return rom;
                    }
                }
            }

            return null;
        }


        protected (Stream?, string) Open(FileInfo file, bool matchExtension = false)
        {
            if (IsCompressed(file))
            {
                return OpenArchive(file, matchExtension);
            }

            if (matchExtension && !system.SupportedExtensions.Contains(file.Extension))
            {
                return (null, "");
            }

            _openStream = file.OpenRead();
            return (_openStream, file.Extension);
        }


        protected void Close()
        {
            _openStream?.Dispose();
            _openArchive?.Dispose();
        }

        protected static long GetFileSize(FileInfo file)
        {
            if (IsCompressed(file))
            {
                using var archive = ZipFile.OpenRead(file.FullName);
                return archive.Entries.FirstOrDefault()?.Length ?? 0;
            }

            return file.Length;
        }

        private (Stream?, string) OpenArchive(FileInfo file, bool matchExtension = false)
        {
            var cachedExtension = _archivesInnerExtensionCache.GetValueOrDefault(file.FullName);

            if (matchExtension && cachedExtension != null && !system.SupportedExtensions.Contains(cachedExtension))
            {
                return (null, "");
            }

            _openArchive = ZipFile.OpenRead(file.FullName);

            var firstEntry = _openArchive.Entries.FirstOrDefault();
            if (firstEntry == null)
            {
                Close();
                return (null, "");
            }

            var extension = Path.GetExtension(firstEntry.FullName).ToLower();
            _archivesInnerExtensionCache[file.FullName] = extension;

            if (matchExtension && !system.SupportedExtensions.Contains(extension))
            {
                Close();
                return (null, "");
            }

            _openStream = firstEntry.Open();

            return (_openStream, extension);
        }
        private static bool IsCompressed(FileInfo file) => file.Extension == ".zip" || file.Extension == ".7z";

        private static Dictionary<string, string> _archivesInnerExtensionCache = [];

        private ZipArchive? _openArchive;
        private Stream? _openStream;
    }
}
