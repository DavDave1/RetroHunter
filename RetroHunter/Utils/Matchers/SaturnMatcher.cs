using RetroHunter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using static RetroHunter.Models.GameSystemData;


namespace RetroHunter.Utils.Matchers
{
    public class SegaSaturnAndCDMatcher(GameSystem system, Dictionary<string, Rom> romsDictionary) : MatcherBase(system, romsDictionary)
    {
        public override (Rom?, List<string>) FindRom(FileInfo file)
        {
            if (!MatchesExtension(file))
            {
                return (null, [file.FullName]);
            }

            using var cdImage = new DiskReader.DiskImage(file.FullName);

            var volumeHeader = cdImage.GetVolumeHeader();

            if (volumeHeader == null)
            {
                return (null, [file.FullName]);
            }


            if (_gameSystem.GameSystemType == GameSystemType.Saturn)
            {
                string identifier = Encoding.ASCII.GetString(volumeHeader.AsSpan()[..SATURN_IDENTIFIER.Length]);
                if (identifier != SATURN_IDENTIFIER)
                {
                    return (null, [file.FullName]);
                }
            }
            else if (_gameSystem.GameSystemType == GameSystemType.SegaCD)
            {
                string identifier = Encoding.ASCII.GetString(volumeHeader.AsSpan()[..SEGACD_IDENTIFIER.Length]);
                if (identifier != SEGACD_IDENTIFIER)
                {
                    return (null, [file.FullName]);
                }
            }
            else
            {
                throw new ArgumentException($"Invalid game system type {_gameSystem.GameSystemType} for SegaSaturnCDMatcher");
            }

            var hash = MD5.HashData(volumeHeader[..512]);
            var rom = MatchRomByHash(Convert.ToHexStringLower(hash));

            return (rom, cdImage.GetAllTrackFiles());
        }

        private static readonly string SEGACD_IDENTIFIER = "SEGADISCSYSTEM";
        private static readonly string SATURN_IDENTIFIER = "SEGA SEGASATURN";

        private readonly GameSystem _gameSystem = system;
    }
}
