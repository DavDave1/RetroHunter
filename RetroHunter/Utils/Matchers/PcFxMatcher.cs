using DiskReader;
using RetroHunter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace RetroHunter.Utils.Matchers
{
    public class PcFxMatcher(GameSystem system, Dictionary<string, Rom> romsDictionary) : PcEngineCDMatcher(system, romsDictionary)
    {
        public override (Rom?, List<string>) FindRom(FileInfo file)
        {
            if (!MatchesExtension(file))
            {
                return (null, [file.FullName]);
            }

            using var cdImage = new DiskImage(file.FullName);

            var header = new byte[32];

            uint trackIdx = 0;
            bool headerFound = false;
            while (cdImage.ReadDataRaw(header, 0, trackIdx))
            {
                cdImage.ReadDataRaw(header, 0, 0);

                var headerId = Encoding.ASCII.GetString(header[..HEADER_ID.Length]);

                if (headerId == HEADER_ID)
                {
                    headerFound = true;
                    break;
                }

                trackIdx++;
            }

            // PC-FX header not found, check if this can be identified as PC-Engine CD ROM
            if (!headerFound)
            {
                return base.FindRom(file);
            }

            var volumeHeader = new byte[128];
            cdImage.ReadDataRaw(volumeHeader, 1, trackIdx);


            var bootCodeSector = BitConverter.ToUInt32(volumeHeader[32..36]);
            var bootCodeLength = BitConverter.ToUInt32(volumeHeader[36..40]);

            var bootCodeData = new byte[bootCodeLength * 2048];

            bool readOk = cdImage.ReadDataRaw(bootCodeData, bootCodeSector, 0);

            if (!readOk)
            {
                return (null, [file.FullName]);
            }

            var hash = MD5.HashData(bootCodeData);

            var rom = MatchRomByHash(Convert.ToHexStringLower(hash));

            return (rom, cdImage.GetAllTrackFiles());
        }

        private static readonly string HEADER_ID = "PC-FX:Hu_CD-ROM";
    }

}
