using RaSetMaker.Models;
using RaSetMaker.Utils;
using RaSetMaker.Utils.Matchers;

namespace RaSetMaker.Tests.MatchersTests
{
    public class NesMatcherTests : MatcherTestBase
    {
        [Theory]
        [InlineData("../../../TestRoms/nes_headered.zip", "29e5e1a5f8b400773ef9d959044456b2")]
        public void RomWithHeaderMatchesHash(string filePath, string expectedHash)
        {
            var sys = GetGameSystemByName("NES/Famicom");
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var foundRom = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);

        }
    }
}
