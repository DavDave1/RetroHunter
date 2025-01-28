using DiskReader;
using RaSetMaker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace RaSetMaker.Utils.Matchers
{
    public class PspMatcher(GameSystem system) : MatcherBase(system)
    {
        public override (Rom?, List<string>) FindRom(FileInfo file)
        {
            var cdImage = new Iso9660Image();

            if (!cdImage.Load(file.FullName))
            {
                return (null, [file.FullName]);
            }

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


        private class PsxExeHeader
        {
            public int FileSize { get; private set; }

            public bool IsValid { get; private set; }

            public PsxExeHeader(byte[] data)
            {
                if (data.Length < 0x800)
                {
                    return;
                }

                if (!data[..HEADER_ID.Length].SequenceEqual(HEADER_ID))
                {
                    return;
                }

                IsValid = true;
                FileSize = BitConverter.ToInt32(data[28..32]);
            }

            private readonly byte[] HEADER_ID = [0x50, 0x53, 0x2D, 0x58, 0x20, 0x45, 0x58, 0x45]; // PS-X EXE
        }
    }
}
