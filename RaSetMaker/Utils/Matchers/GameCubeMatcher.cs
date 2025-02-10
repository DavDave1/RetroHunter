using RaSetMaker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace RaSetMaker.Utils.Matchers
{
    // From https://github.com/RetroAchievements/rcheevos/blob/cd90a87e50458a1520b6eb46d4b2eb97661ad1e1/src/rhash/hash.c#L2287
    public class GameCubeMatcher(GameSystem system) : MatcherBase(system)
    {
        public override (Rom?, List<string>) FindRom(FileInfo file)
        {
            var (fileStream, _) = Open(file, true);

            if (fileStream == null)
            {
                return (null, []);
            }

            fileStream.Seek(0x1C, SeekOrigin.Begin);

            var buffer = new byte[4];

            fileStream.ReadExactly(buffer);

            if (!buffer.SequenceEqual(DISC_IDENTIFIER))
            {
                return (null, []);
            }

            fileStream.Seek(BASE_HEADER_SIZE + 0x14, SeekOrigin.Begin);

            fileStream.ReadExactly(buffer);
            var apploaderBodySize = BitConverter.ToUInt32(buffer.Reverse().ToArray());

            fileStream.ReadExactly(buffer);
            var apploaderTrailerSize = BitConverter.ToUInt32(buffer.Reverse().ToArray());

            var diskHeaderSize = BASE_HEADER_SIZE + APPLOADER_HEADER_SIZE + apploaderBodySize + apploaderTrailerSize;
            diskHeaderSize = Math.Min(diskHeaderSize, MAX_HEADER_SIZEE);

            var headerBuffer = new byte[diskHeaderSize];

            fileStream.Seek(0, SeekOrigin.Begin);
            fileStream.ReadExactly(headerBuffer);

            var dolInfoAddress = BitConverter.ToUInt32(headerBuffer[0x420..0x424].Reverse().ToArray());

            fileStream.Seek(dolInfoAddress, SeekOrigin.Begin);

            var addrBuffer = new byte[0xD8];

            fileStream.ReadExactly(addrBuffer);

            MD5 hasher = MD5.Create();

            hasher.TransformBlock(headerBuffer, 0, headerBuffer.Length, null, 0);

            for (int i = 0; i < 18; ++i)
            {
                var offsetStartIndex = i * 4;
                var offsetEndIndex = offsetStartIndex + 4;

                var dolOffset = BitConverter.ToUInt32(addrBuffer[offsetStartIndex..offsetEndIndex].Reverse().ToArray());

                var sizeStartIndex = 0x90 + i * 4;
                var sizeEndIndex = sizeStartIndex + 4;

                var dolSize = BitConverter.ToUInt32(addrBuffer[sizeStartIndex..sizeEndIndex].Reverse().ToArray());

                var dolBuffer = new byte[dolSize];


                fileStream.Seek(dolOffset, SeekOrigin.Begin);
                fileStream.ReadExactly(dolBuffer);

                if (i == 17)
                {
                    hasher.TransformFinalBlock(dolBuffer, 0, (int)dolSize);

                }
                else
                {
                    hasher.TransformBlock(dolBuffer, 0, (int)dolSize, null, 0);
                }
            }

            var hash = Convert.ToHexStringLower(hasher.Hash ?? []);

            var rom = MatchRomByHash(hash);

            return (rom, [file.FullName]);
        }


        private static readonly byte[] DISC_IDENTIFIER = [0xC2, 0x33, 0x9F, 0x3D];
        private const int BASE_HEADER_SIZE = 0x2440;
        private const int APPLOADER_HEADER_SIZE = 0x20;

        private const int MAX_HEADER_SIZEE = 1024 * 1024;

    }

}
