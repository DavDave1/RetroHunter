using DiskReader;
using RaSetMaker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace RaSetMaker.Utils.Matchers
{
    public class ThreeDOMatcher(GameSystem system) : MatcherBase(system)
    {
        public override (Rom?, List<string>) FindRom(FileInfo file)
        {
            var cdImage = new Iso9660Image(file.FullName);

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
