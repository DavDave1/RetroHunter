using Avalonia.DesignerSupport.Remote.HtmlTransport;
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
    public class JaguarCDMatcher(GameSystem system, Dictionary<string, Rom> romsDictionary) : MatcherBase(system, romsDictionary)
    {
        public override (Rom?, List<string>) FindRom(FileInfo file)
        {
            if (!MatchesExtension(file))
            {
                return (null, [file.FullName]);
            }

            using var cdImage = new DiskImage(file.FullName, DiskImage.ReadMode.TreatAudioTracksAsData);

            var buffer = new byte[2352];
            cdImage.ReadDataRaw(buffer, 0, 0, 2);

            // Search header start by matching header id
            int headerIDStart = 64;
            bool byteswapped = false;

            for (; headerIDStart < buffer.Length - HEADER_ID.Length - 12; headerIDStart++)
            {
                var headerId = Encoding.ASCII.GetString(buffer, headerIDStart, HEADER_ID.Length);

                if (headerId == HEADER_ID)
                {
                    break;
                }

                if (headerId == HEADER_ID_BS)
                {
                    byteswapped = true;
                    break;
                }
            }

            // Boot code size is 4 bytes after header id
            int bootCodeSizeStart = headerIDStart + HEADER_ID.Length + 4;
            uint bootCodeSize = ParseBigEndian(buffer[bootCodeSizeStart..], byteswapped);

            // Boot code starts after boot code size, hash final part of sector 0 buffer
            int bootCodeStart = bootCodeSizeStart + 4;

            if (byteswapped)
            {
                ByteSwapBuffer(buffer);
            }

            var hasher = MD5.Create();
            hasher.Initialize();

            hasher.TransformBlock(buffer, bootCodeStart, buffer.Length - bootCodeStart, null, 0);

            // And continue reading boot code from next sectors
            uint sizeToRead = bootCodeSize - (uint)(buffer.Length - bootCodeStart);
            uint sector = 1;
            while (sizeToRead > 0)
            {
                uint readSize = Math.Min(2352, sizeToRead);

                bool readOk = cdImage.ReadDataRaw(buffer, sector, 0, 2);

                if (!readOk)
                {
                    return (null, [file.FullName]);
                }

                if (byteswapped)
                {
                    ByteSwapBuffer(buffer);
                }

                sizeToRead -= readSize;

                if (sizeToRead == 0)
                {
                    hasher.TransformFinalBlock(buffer, 0, (int)readSize);

                }
                else
                {
                    hasher.TransformBlock(buffer, 0, (int)readSize, null, 0);

                }

                sector++;
            }

            var hash = Convert.ToHexStringLower(hasher.Hash!);

            var rom = MatchRomByHash(hash);

            return (rom, cdImage.GetAllTrackFiles());
        }

        private static readonly string HEADER_ID = "ATARI APPROVED DATA HEADER ATRI ";
        private static readonly string HEADER_ID_BS = "TARA IPARPVODED TA AEHDAREA RT I";

        private static uint ParseBigEndian(byte[] buffer, bool byteswapped)
        {
            return (uint)
                (byteswapped ?
                    buffer[1] << 24 | buffer[0] << 16 | buffer[3] << 8 | buffer[2]
                : buffer[0] << 24 | buffer[1] << 16 | buffer[2] << 8 | buffer[3]);

        }

        private static void ByteSwapBuffer(byte[] buffer)
        {
            for (int i = 1; i < buffer.Length; i += 2)
            {
                var tmp = buffer[i];
                buffer[i] = buffer[i - 1];
                buffer[i - 1] = tmp;
            }
        }
    }

}
