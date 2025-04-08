using RetroHunter.Models;
using System.Collections.Generic;
using System.IO;

namespace RetroHunter.Utils.Matchers
{
    public class NullMatcher(GameSystem system) : MatcherBase(system)
    {
        public override (Rom?, List<string>) FindRom(FileInfo file)
        {
            return (null, [file.FullName]);
        }
    }
}
