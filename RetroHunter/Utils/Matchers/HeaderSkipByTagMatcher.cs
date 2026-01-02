using RetroHunter.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace RetroHunter.Utils.Matchers
{
    public abstract class HeaderSkipByTagMatcher(GameSystem system, Dictionary<string, Rom> romsDictionary) : Md5Matcher(system, romsDictionary)
    {
        public abstract int SkipSize { get; }

        public abstract List<byte[]> HeaderTags { get; }

        public override (Rom?, List<string>) FindRom(FileInfo file)
        {
            var (fileStream, extension) = Open(file, true);
            if (fileStream == null)
            {
                return (null, [file.FullName]);
            }

            bool hasHeader = CheckHeader(fileStream);

            if (hasHeader)
            {
                for (int i = 0; i < SkipSize; i++)
                {
                    fileStream.ReadByte();
                }
            }
            else
            {
                Close();
                (fileStream, extension) = Open(file, true);
            }

            var hash = ComputeHash(fileStream!, extension);
            Close();

            var rom = MatchRomByHash(hash);

            return (rom, [file.FullName]);
        }

        private bool CheckHeader(Stream fileStream)
        {
            var buffer = new byte[HeaderTags[0].Length];
            fileStream.ReadExactly(buffer, 0, buffer.Length);

            var readHeaderStr = Encoding.Default.GetString(buffer);

            return HeaderTags.Any(tag => buffer.SequenceEqual(tag));
        }
    }
}
