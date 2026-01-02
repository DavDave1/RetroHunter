using DiskReader;
using RetroHunter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace RetroHunter.Utils.Matchers
{
    public class NeoGeoCDMatcher(GameSystem system, Dictionary<string, Rom> romsDictionary) : MatcherBase(system, romsDictionary)
    {
        public override (Rom?, List<string>) FindRom(FileInfo file)
        {
            if (!_system.SupportedExtensions.Contains(file.Extension))
            {
                return (null, [file.FullName]);
            }

            using var cdImage = new DiskImage(file.FullName);

            var iplData = cdImage.ReadFile("IPL.TXT");
            if (iplData == null)
            {
                return (null, [file.FullName]);
            }

            var prgBuffer = Array.Empty<byte>();
            foreach (var line in Encoding.UTF8.GetString(iplData).Split('\n'))
            {
                var fileName = line.Split(',').FirstOrDefault();

                if (fileName == null || !fileName.EndsWith(".PRG"))
                {
                    continue;
                }

                var prgData = cdImage.ReadFile(fileName);

                if (prgData == null)
                {
                    return (null, [file.FullName]);
                }

                prgBuffer = [.. prgBuffer, .. prgData];
            }

            var hash = MD5.HashData(prgBuffer);

            var rom = MatchRomByHash(Convert.ToHexStringLower(hash));

            return (rom, cdImage.GetAllTrackFiles());
        }

        private readonly GameSystem _system = system;
    }


}
