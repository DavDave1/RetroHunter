using RaSetMaker.Models;
using System.Collections.Generic;


namespace RaSetMaker.Utils.Matchers
{
    public class Atari7800Matcher(GameSystem system) : HeaderSkipByTagMatcher(system)
    {
        // \1ATARI7800
        private static readonly byte[] HEADER_TAG = [0x01, 0x41, 0x54, 0x41, 0x52, 0x49, 0x37, 0x38, 0x30, 0x30];
        public override int SkipSize => 128 - HEADER_TAG.Length;

        public override List<byte[]> HeaderTags => [HEADER_TAG];
    }
}
