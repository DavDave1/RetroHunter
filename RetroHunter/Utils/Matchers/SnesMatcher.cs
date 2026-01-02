using RetroHunter.Models;
using System.Collections.Generic;

namespace RetroHunter.Utils.Matchers
{
    public class SnesMatcher(GameSystem system, Dictionary<string, Rom> romsDictionary) : HeaderSkipBySizeMatcher(system, romsDictionary)
    {
        public override int MultipleSize => 8192;
    }
}
