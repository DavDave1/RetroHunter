
using RaSetMaker.Utils;

namespace RaSetMaker.Tests.MatchersTests
{
    public class Ps1MatcherTests : MatcherTestBase
    {
        [Theory]
        [InlineData("../../../TestRoms/ps1/007 Racing (USA).cue", "3620a316e7ce463e604d91540840df62")]
        public void RomMatchesHash(string filePath, string expectedHash)
        {
            var sys = GetGameSystemByName("PlayStation");
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var (foundRom, _) = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);

        }

        [Theory]
        [InlineData("../../../TestRoms/ps1/007 Racing (USA).chd", "3620a316e7ce463e604d91540840df62")]
        public void CompressedRomMatchesHash(string filePath, string expectedHash)
        {
            var sys = GetGameSystemByName("PlayStation");
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var (foundRom, _) = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);

        }
    }
}
