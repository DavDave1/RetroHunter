using RaSetMaker.Models;
using RaSetMaker.Utils;
using RaSetMaker.Utils.Matchers;

namespace RaSetMaker.Tests.MatchersTests
{
    public class SnesMatcherTests : MatcherTestBase
    {
        [Theory]
        [InlineData("../../../TestRoms/snes.zip", "c638c1175840c6640d897951daa73637")]
        public void RomMatchesHash(string filePath, string expectedHash)
        {
            var sys = GetGameSystemByName("SNES/Super Famicom");
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var foundRom = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);

        }
    }
}
