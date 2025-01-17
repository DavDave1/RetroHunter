using RaSetMaker.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace RaSetMaker.Utils.Matchers
{
    public class PcEngineMatcher(GameSystem system) : HeaderSkipBySizeMatcher(system)
    {
        public override int MultipleSize => 131072;
    }
}
