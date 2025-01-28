using RaSetMaker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RaSetMaker.Utils.Matchers
{
    public class FileNameHashMatcher(GameSystem system) : MatcherBase(system)
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
