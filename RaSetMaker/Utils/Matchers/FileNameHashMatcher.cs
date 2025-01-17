using RaSetMaker.Models;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RaSetMaker.Utils.Matchers
{
    public class FileNameHashMatcher(GameSystem system) : MatcherBase(system)
    {
        public override Rom? FindRom(FileInfo file)
        {
            var baseName = Path.GetFileNameWithoutExtension(file.Name);

            var hash = Convert.ToHexStringLower(_hasher.ComputeHash(Encoding.UTF8.GetBytes(baseName)));

            return base.MatchRomByHash(hash);
        }

        private readonly MD5 _hasher = MD5.Create();
    }
}
