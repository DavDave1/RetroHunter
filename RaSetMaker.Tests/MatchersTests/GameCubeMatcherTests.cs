
using RaSetMaker.Utils;

namespace RaSetMaker.Tests.MatchersTests
{
    public class GameCubeMatcherTests : MatcherTestBase
    {
        [Theory]
        [InlineData("../../../TestRoms/Animal Crossing (USA).iso", "4f69c7886162509baa0882062bb2e1c8")]
        public void RomMatchesHash(string filePath, string expectedHash)
        {
            var sys = GetGameSystemByName("Game Cube");
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var foundRom = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom.Item1);
            Assert.Equal(expectedHash, foundRom.Item1.Hash);

        }
    }
}
