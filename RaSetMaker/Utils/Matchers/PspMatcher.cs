using DiskReader;
using RaSetMaker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace RaSetMaker.Utils.Matchers
{
    public class PspMatcher(GameSystem system) : MatcherBase(system)
    {
        public override (Rom?, List<string>) FindRom(FileInfo file)
        {
            if (!system.SupportedExtensions.Contains(file.Extension))
            {
                return (null, [file.FullName]);
            }

            using var cdImage = new DiskImage(file.FullName);

            var paramSfoData = cdImage.ReadFile(@"PSP_GAME\PARAM.SFO");

            if (paramSfoData == null)
            {
                return (null, [file.FullName]);
            }

            var exeData = cdImage.ReadFile(@"PSP_GAME\SYSDIR\EBOOT.BIN");

            if (exeData == null)
            {
                return (null, [file.FullName]);
            }

            var hash = Convert.ToHexStringLower(_hasher.ComputeHash([.. paramSfoData, .. exeData]));
            var rom = MatchRomByHash(hash);

            return (rom, [file.FullName]);
        }

        protected static readonly MD5 _hasher = MD5.Create();
    }
}
