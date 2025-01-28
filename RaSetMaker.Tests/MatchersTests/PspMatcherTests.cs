
using RaSetMaker.Utils;

namespace RaSetMaker.Tests.MatchersTests
{
    public class PspMatcherTests : MatcherTestBase
    {
        [Theory]
        [InlineData("../../../TestRoms/psp.iso", "3cd66bf66e631a8d90c97a2f7628bd2a")]
        public void RomMatchesHash(string filePath, string expectedHash)
        {
            var sys = GetGameSystemByName("PlayStation Portable");
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var (foundRom, _) = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);

        }
    }
}
