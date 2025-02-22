using RaSetMaker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using static RaSetMaker.Models.GameSystemData;


namespace RaSetMaker.Utils.Matchers
{
    public class SegaSaturnAndCDMatcher(GameSystem system) : MatcherBase(system)
    {
        public override (Rom?, List<string>) FindRom(FileInfo file)
        {
            if (!system.SupportedExtensions.Contains(file.Extension))
            {
                return (null, [file.FullName]);
            }

            var cdImage = new DiskReader.DiskImage(file.FullName);

            var volumeHeader = cdImage.GetVolumeHeader();

            if (volumeHeader == null)
            {
                return (null, [file.FullName]);
            }


            if (system.GameSystemType == GameSystemType.Saturn)
            {
                string identifier = Encoding.ASCII.GetString(volumeHeader.AsSpan()[..SATURN_IDENTIFIER.Length]);
                if (identifier != SATURN_IDENTIFIER)
                {
                    return (null, [file.FullName]);
                }
            }
            else if (system.GameSystemType == GameSystemType.SegaCD)
            {
                string identifier = Encoding.ASCII.GetString(volumeHeader.AsSpan()[..SEGACD_IDENTIFIER.Length]);
                if (identifier != SEGACD_IDENTIFIER)
                {
                    return (null, [file.FullName]);
                }
            }
            else
            {
                throw new ArgumentException($"Invalid game system type {system.GameSystemType} for SegaSaturnCDMatcher");
            }

            var hash = MD5.HashData(volumeHeader[..512]);
            var rom = MatchRomByHash(Convert.ToHexStringLower(hash));

            return (rom, cdImage.GetAllTrackFiles());
        }

        private static readonly string SEGACD_IDENTIFIER = "SEGADISCSYSTEM";
        private static readonly string SATURN_IDENTIFIER = "SEGA SEGASATURN";
    }
}
