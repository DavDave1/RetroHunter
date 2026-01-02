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
    public class Ps2Matcher(GameSystem system, Dictionary<string, Rom> romsDictionary) : MatcherBase(system, romsDictionary)
    {
        public override (Rom?, List<string>) FindRom(FileInfo file)
        {
            if (!MatchesExtension(file))
            {
                return (null, [file.FullName]);
            }

            using var cdImage = new DiskImage(file.FullName);

            var systemCnfData = cdImage.ReadFile("SYSTEM.CNF");

            if (systemCnfData == null)
            {
                return (null, [file.FullName]);
            }

            var systemCnf = Encoding.UTF8.GetString(systemCnfData);

            var bootLine = systemCnf.Split('\n').FirstOrDefault();

            if (bootLine == null)
            {
                return (null, [file.FullName]);
            }

            var exeFilePath = bootLine.Split("=").Last().Trim();

            var exeFileName = exeFilePath.Replace("cdrom0:\\", "").Replace(";1", "");

            var exeFileData = cdImage.ReadFile(exeFileName);

            if (exeFileData == null)
            {
                return (null, [file.FullName]);
            }

            var exeNameToHash = Encoding.ASCII.GetBytes(exeFileName);

            _hasher.Initialize();
            _hasher.TransformBlock(exeNameToHash, 0, exeNameToHash.Length, null, 0);
            _hasher.TransformFinalBlock(exeFileData, 0, exeFileData.Length);

            var rom = MatchRomByHash(Convert.ToHexStringLower(_hasher.Hash!));

            return (rom, cdImage.GetAllTrackFiles());
        }

        protected static readonly MD5 _hasher = MD5.Create();
    }
}
