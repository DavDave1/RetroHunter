using RaSetMaker.Models;
using System.IO;

namespace RaSetMaker.Utils.Matchers
{
    public class NullMatcher(GameSystem system) : MatcherBase(system)
    {
        public override Rom? FindRom(FileInfo file)
        {
            return null;
        }
    }
}
