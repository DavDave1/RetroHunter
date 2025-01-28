using RaSetMaker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RaSetMaker.Utils.Matchers
{
    public class ArduboyMatcher(GameSystem system) : Md5Matcher(system)
    {
        public override (Rom?, List<string>) FindRom(FileInfo file)
        {
            _romSize = GetFileSize(file);
            return base.FindRom(file);
        }

        protected override bool CacheHash => false;

        protected override string ComputeHash(Stream fileStream, string extension)
        {
            var romData = new byte[_romSize];

            fileStream.ReadExactly(romData, 0, romData.Length);

            List<byte> normalizedData = [];
            for(int i = 0; i < romData.Length; i++)
            {
                // replace \r\n with \n
                if ( i < romData.Length - 1 && romData[i] == 0x0D && romData[i + 1] == 0x0A)
                {
                    normalizedData.Add(0x0A);
                    ++i;
                }
                else if (romData[i] == 0x0D) // replace \r with \n
                {
                    normalizedData.Add(0x0A);
                }
                else
                {
                    normalizedData.Add(romData[i]);
                }
            }

            return Convert.ToHexStringLower(_hasher.ComputeHash([..normalizedData]));
        }

        private long _romSize;
    }
}
