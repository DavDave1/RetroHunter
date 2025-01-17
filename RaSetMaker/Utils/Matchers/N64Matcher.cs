using RaSetMaker.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace RaSetMaker.Utils.Matchers
{
    public class N64Matcher(GameSystem system) : Md5Matcher(system)
    {
        protected override string ComputeHash(Stream fileStream, string extension)
        {
            if (extension == ".z64")
            {
                return base.ComputeHash(fileStream, extension);
            }
            else if (extension == ".v64") // byte swapped
            {
                var data = new List<byte>();

                while (true)
                {
                    int b0 = fileStream.ReadByte();
                    int b1 = fileStream.ReadByte();

                    if (b0 == -1 || b1 == -1)
                    {
                        break;
                    }
                    data.Add((byte)b1);
                    data.Add((byte)b0);
                }

                return Convert.ToHexStringLower(_hasher.ComputeHash([.. data]));
            }
            else if (extension == ".n64")
            {
                var data = new List<byte>();

                while (true)
                {
                    int b0 = fileStream.ReadByte();
                    int b1 = fileStream.ReadByte();
                    int b2 = fileStream.ReadByte();
                    int b3 = fileStream.ReadByte();

                    if (b0 == -1 || b1 == -1 || b2 == -1 || b3 == -1)
                    {
                        break;
                    }

                    data.Add((byte)b3);
                    data.Add((byte)b2);
                    data.Add((byte)b1);
                    data.Add((byte)b0);
                }

                return Convert.ToHexStringLower(_hasher.ComputeHash([.. data]));
            }

            return "";
        }
        protected override bool CacheHash => false;

    }

}
