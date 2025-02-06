
using RaSetMaker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RaSetMaker.Utils.Matchers
{
    public class NdsMatcher(GameSystem system) : Md5Matcher(system)
    {
        public override (Rom?, List<string>) FindRom(FileInfo file)
        {
            _romSize = GetFileSize(file);
            return base.FindRom(file);
        }

        // From https://github.com/libretro/RetroArch/blob/beceb88cd7d722b86057898c2f47ecad64a7b233/deps/rcheevos/src/rhash/hash.c#L2186
        protected override string ComputeHash(Stream fileStream, string extension)
        {
            const int NDS_HEADER_SIZE = 0x160;

            if (_romSize < NDS_HEADER_SIZE)
            {
                return "";
            }

            var romData = new byte[_romSize];
            fileStream.ReadExactly(romData, 0, romData.Length);

            int headerStart = 0;

            if (romData[0] == 0x2E && romData[1] == 0x00 && romData[2] == 0x00 && romData[3] == 0xEA &&
                romData[0xB0] == 0x44 && romData[0xB1] == 0x46 && romData[0xB2] == 0x96 && romData[0xB3] == 0)
            {
                headerStart = 0x22;
            }

            var headerEnd = headerStart + NDS_HEADER_SIZE;

            var header = romData[headerStart..headerEnd];

            int arm9AddrStart = header[0x20] | (header[0x21] << 8) | (header[0x22] << 16) | (header[0x23] << 24);
            int arm9Size = header[0x2C] | (header[0x2D] << 8) | (header[0x2E] << 16) | (header[0x2F] << 24);
            int arm9AddrEnd = arm9AddrStart + arm9Size;
            int arm7AddrStart = header[0x30] | (header[0x31] << 8) | (header[0x32] << 16) | (header[0x33] << 24);
            int arm7Size = header[0x3C] | (header[0x3D] << 8) | (header[0x3E] << 16) | (header[0x3F] << 24);
            int arm7AddrEnd = arm7AddrStart + arm7Size;
            int iconAddrStart = header[0x68] | (header[0x69] << 8) | (header[0x6A] << 16) | (header[0x6B] << 24);
            int iconAddrEnd = iconAddrStart + 0xA00;

            /* sanity check - code blocks are typically less than 1MB each - assume not a DS ROM */
            if (arm9Size + arm7Size > 16 * 1024 * 1024)
            {
                return "";
            }

            List<byte> dataToHash = [.. header];
            dataToHash.AddRange(romData[arm9AddrStart..arm9AddrEnd]);
            dataToHash.AddRange(romData[arm7AddrStart..arm7AddrEnd]);
            dataToHash.AddRange(romData[iconAddrStart..iconAddrEnd]);

            return Convert.ToHexStringLower(_hasher.ComputeHash([.. dataToHash]));

        }
        protected override bool CacheHash => false;

        private long _romSize;
    }

}
