using RetroHunter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RetroHunter.Utils.Matchers
{
    public class FileNameHashMatcher(GameSystem system, Dictionary<string, Rom> romsDictionary) : MatcherBase(system, romsDictionary)
    {
        public override (Rom?, List<string>) FindRom(FileInfo file)
        {
            var baseName = Path.GetFileNameWithoutExtension(file.Name);

            var hash = Convert.ToHexStringLower(_hasher.ComputeHash(Encoding.UTF8.GetBytes(baseName)));

            return (MatchRomByHash(hash), [file.FullName]);
        }

        private readonly MD5 _hasher = MD5.Create();
    }
}
