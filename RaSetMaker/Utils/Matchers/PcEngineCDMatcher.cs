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
    public class PcEngineCDMatcher(GameSystem system) : MatcherBase(system)
    {
        public override (Rom?, List<string>) FindRom(FileInfo file)
        {
            var cdImage = new DiskImage(file.FullName);

            var header = new byte[128];
            cdImage.ReadDataRaw(header, 1, 0);

            int headerIDStart = 32;
            int headerIDEnd = 32 + HEADER_ID.Length;

            var headerId = Encoding.ASCII.GetString(header[headerIDStart..headerIDEnd]);

            if (headerId != HEADER_ID)
            {
                return (null, [file.FullName]);
            }


            var bootCodeSector = (uint)(header[0] << 16 | header[1] << 8 | header[2]);
            var bootCodeLength = header[3];

            var bootCodeData = new byte[bootCodeLength * 2048];

            bool readOk = cdImage.ReadDataRaw(bootCodeData, bootCodeSector, 0);

            if (!readOk)
            {
                return (null, [file.FullName]);
            }

            var hasher = MD5.Create();
            hasher.Initialize();
            hasher.TransformBlock(header, 106, 22, null, 0); // Disc title is in last 22 bytes of 128 bytes header
            hasher.TransformFinalBlock(bootCodeData, 0, bootCodeData.Length);


            var rom = MatchRomByHash(Convert.ToHexStringLower(hasher.Hash!));

            return (rom, cdImage.GetAllTrackFiles());
        }

        private static readonly string HEADER_ID = "PC Engine CD-ROM SYSTEM";
    }

}
