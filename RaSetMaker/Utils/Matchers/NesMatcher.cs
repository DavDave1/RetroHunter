using RaSetMaker.Models;
using System.Collections.Generic;

namespace RaSetMaker.Utils.Matchers
{
    public class NesMatcher(GameSystem system) : HeaderSkipByTagMatcher(system)
    {
        private static readonly byte [] NES_HEADER_TAG = [0x4E, 0x45, 0x53, 0x1A];  // NES\1a
        private static readonly byte [] FDS_HEADER_TAG = [0x46, 0x44, 0x53, 0x1A];  // FDS\1a

        public override int SkipSize => 16 - NES_HEADER_TAG.Length;

        public override List<byte[]> HeaderTags => [NES_HEADER_TAG, FDS_HEADER_TAG];

    }
}
