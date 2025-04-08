using RetroHunter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace RetroHunter.Utils.Matchers
{
    public class DreamcastMatcher(GameSystem system) : MatcherBase(system)
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

            string dcIdentifier = Encoding.ASCII.GetString(volumeHeader.AsSpan()[..16]);
            if (dcIdentifier != DC_IDENTIFIER)
            {
                return (null, [file.FullName]);
            }

            string bootFilename = Encoding.ASCII.GetString(volumeHeader.AsSpan(96..112)).Trim();

            var bootFileData = cdImage.ReadFile(bootFilename);

            if (bootFileData == null)
            {
                return (null, [file.FullName]);
            }

            var hasher = MD5.Create();
            hasher.Initialize();
            hasher.TransformBlock(volumeHeader, 0, 256, null, 0);
            hasher.TransformFinalBlock(bootFileData, 0, bootFileData.Length);

            var rom = MatchRomByHash(Convert.ToHexStringLower(hasher.Hash!));

            return (rom, cdImage.GetAllTrackFiles());
        }

        private static readonly string DC_IDENTIFIER = "SEGA SEGAKATANA ";

    }
}
