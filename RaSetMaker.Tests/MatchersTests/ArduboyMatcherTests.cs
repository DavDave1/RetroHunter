
using RaSetMaker.Utils;

namespace RaSetMaker.Tests.MatchersTests
{
    public class ArduboyMatcherTests : MatcherTestBase
    {
        [Theory]
        [InlineData("../../../TestRoms/arduboy.zip", "cee7f24fab74cef92ff1e03cd76d38cb")]
        public void RomMatchesHash(string filePath, string expectedHash)
        {
            var sys = GetGameSystemByName("Arduboy");
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var foundRom = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);

        }
    }
}
