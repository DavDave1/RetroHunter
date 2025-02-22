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
    public class Ps1Matcher(GameSystem system) : MatcherBase(system)
    {
        public override (Rom?, List<string>) FindRom(FileInfo file)
        {
            if (!system.SupportedExtensions.Contains(file.Extension))
            {
                return (null, [file.FullName]);
            }

            var cdImage = new DiskImage(file.FullName);

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

            var exeFileName = exeFilePath.Replace("cdrom:\\", "").Replace(";1", "");

            var exeFileData = cdImage.ReadFile(exeFileName);

            if (exeFileData == null)
            {
                return (null, [file.FullName]);
            }

            var header = new PsxExeHeader(exeFileData);

            if (!header.IsValid)
            {
                return (null, [file.FullName]);
            }

            var exeNameToHash = Encoding.ASCII.GetBytes(exeFileName);

            // https://github.com/stenzek/duckstation/blob/master/src/core/achievements.cpp#L315
            var exeDataSizeToHash = Math.Min(header.FileSize + 2048, exeFileData.Length);

            _hasher.Initialize();
            _hasher.TransformBlock(exeNameToHash, 0, exeNameToHash.Length, null, 0);
            _hasher.TransformFinalBlock(exeFileData, 0, exeDataSizeToHash);

            var rom = MatchRomByHash(Convert.ToHexStringLower(_hasher.Hash!));

            return (rom, cdImage.GetAllTrackFiles());
        }

        protected static readonly MD5 _hasher = MD5.Create();


        private class PsxExeHeader
        {
            public int FileSize { get; private set; }

            public bool IsValid { get; private set; }

            public PsxExeHeader(byte[] data)
            {
                if (data.Length < 0x800)
                {
                    return;
                }

                if (!data[..HEADER_ID.Length].SequenceEqual(HEADER_ID))
                {
                    return;
                }

                IsValid = true;
                FileSize = BitConverter.ToInt32(data[28..32]);
            }

            private readonly byte[] HEADER_ID = [0x50, 0x53, 0x2D, 0x58, 0x20, 0x45, 0x58, 0x45]; // PS-X EXE
        }
    }
}
