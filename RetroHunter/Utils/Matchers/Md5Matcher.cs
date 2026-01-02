using RetroHunter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace RetroHunter.Utils.Matchers
{
    public class Md5Matcher(GameSystem system, Dictionary<string, Rom> romsDictionary) : MatcherBase(system, romsDictionary)
    {
        public override (Rom?, List<string>) FindRom(FileInfo file)
        {
            var hash = _hashesCache.GetValueOrDefault(file.FullName);

            if (string.IsNullOrEmpty(hash))
            {
                var (fileStream, extension) = Open(file, true);
                if (fileStream == null)
                {
                    return (null, [file.FullName]);
                }
                hash = ComputeHash(fileStream, extension);
                Close();
            }

            var rom = MatchRomByHash(hash);

            if (rom == null && CacheHash)
            {
                _hashesCache[file.FullName] = hash;
            }

            return (rom, [file.FullName]);
        }

        protected virtual bool CacheHash { get; } = true;

        protected virtual string ComputeHash(Stream fileStream, string extension)
        {
            return Convert.ToHexStringLower(_hasher.ComputeHash(fileStream));
        }

        protected static readonly MD5 _hasher = MD5.Create();

        // filepath to md5 hash cache
        private static readonly Dictionary<string, string> _hashesCache = [];

    }
}
