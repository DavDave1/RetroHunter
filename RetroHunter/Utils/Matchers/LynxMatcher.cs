using RetroHunter.Models;
using System.Collections.Generic;


namespace RetroHunter.Utils.Matchers
{
    public class LynxMatcher(GameSystem system) : HeaderSkipByTagMatcher(system)
    {
        // LYNX\0
        private static readonly byte[] HEADER_TAG = [0x4C, 0x59, 0x4E, 0x58, 0x00];
        public override int SkipSize => 64 - HEADER_TAG.Length;

        public override List<byte[]> HeaderTags => [HEADER_TAG];
    }
}
