using RetroHunter.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace RetroHunter.Utils.Matchers
{
    public class PcEngineMatcher(GameSystem system, Dictionary<string, Rom> romsDictionary) : HeaderSkipBySizeMatcher(system, romsDictionary)
    {
        public override int MultipleSize => 131072;
    }
}
