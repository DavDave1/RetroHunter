using RaSetMaker.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace RaSetMaker.Utils.Matchers
{
    public abstract class HeaderSkipBySizeMatcher(GameSystem system) : Md5Matcher(system)
    {
        public abstract int MultipleSize { get; }

        public override Rom? FindRom(FileInfo file)
        {
           
            var (fileStream, extension) = Open(file);
            if (fileStream == null)
            {
                return null;
            }

            long remainderSize = GetFileSize(file) % MultipleSize;
            bool hasHeader = remainderSize >= 512;

            if (hasHeader)
            {
                byte[] buffer = new byte[512];
                fileStream.ReadExactly(buffer, 0, buffer.Length);
            }

            var hash = ComputeHash(fileStream!, extension);
             Close();

            var rom = MatchRomByHash(hash);

            return rom;
        }
    }
}
