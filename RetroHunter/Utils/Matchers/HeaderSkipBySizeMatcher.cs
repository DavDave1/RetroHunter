using RetroHunter.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace RetroHunter.Utils.Matchers
{
    public abstract class HeaderSkipBySizeMatcher(GameSystem system, Dictionary<string, Rom> romsDictionary) : Md5Matcher(system, romsDictionary)
    {
        public abstract int MultipleSize { get; }

        public override (Rom?, List<string>) FindRom(FileInfo file)
        {

            var (fileStream, extension) = Open(file, true);
            if (fileStream == null)
            {
                return (null, [file.FullName]);
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

            return (rom, [file.FullName]);
        }
    }
}
