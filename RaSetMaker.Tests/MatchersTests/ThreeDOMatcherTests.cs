
using RaSetMaker.Models;
using RaSetMaker.Utils;
using static RaSetMaker.Models.GameSystemData;

namespace RaSetMaker.Tests.MatchersTests
{
    public class ThreeDOMatcherTests : MatcherTestBase
    {
        [Theory]
        [InlineData("../../../TestRoms/3do/Alone in the Dark (USA).cue", "4d7f2e1b2e8b9d9d14f083fae44d9760")]
        public void RomMatchesHash(string filePath, string expectedHash)
        {
            var sys = GetGameSystemByType(GameSystemType.ThreeDo);
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var (foundRom, _) = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);

        }
    }
}
