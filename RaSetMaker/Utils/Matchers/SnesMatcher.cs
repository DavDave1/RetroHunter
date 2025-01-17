using RaSetMaker.Models;

namespace RaSetMaker.Utils.Matchers
{
    public class SnesMatcher(GameSystem system) : HeaderSkipBySizeMatcher(system)
    {
        public override int MultipleSize => 8192;
    }
}
