using DiskReader;
using RetroHunter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using ZstdSharp.Unsafe;

namespace RetroHunter.Utils.Matchers
{
    public class ThreeDOMatcher(GameSystem system, Dictionary<string, Rom> romsDictionary) : MatcherBase(system, romsDictionary)
    {
        public override (Rom?, List<string>) FindRom(FileInfo file)
        {
            if (!MatchesExtension(file))
            {
                return (null, [file.FullName]);
            }

            using var cdImage = new DiskImage(file.FullName);

            var launchMe = cdImage.ReadFile("LaunchMe");

            if (launchMe == null)
            {
                return (null, [file.FullName]);
            }

            var volumeHeader = cdImage.GetVolumeHeader();

            if (volumeHeader == null)
            {
                return (null, [file.FullName]);
            }

            _hasher.Initialize();
            _hasher.TransformBlock(volumeHeader, 0, volumeHeader.Length, null, 0);
            _hasher.TransformFinalBlock(launchMe, 0, launchMe.Length);

            var rom = MatchRomByHash(Convert.ToHexStringLower(_hasher.Hash!));

            return (rom, cdImage.GetAllTrackFiles());
        }

        protected static readonly MD5 _hasher = MD5.Create();
    }
}
