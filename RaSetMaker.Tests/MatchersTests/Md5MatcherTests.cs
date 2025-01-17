using RaSetMaker.Models;
using RaSetMaker.Utils;
using RaSetMaker.Utils.Matchers;

namespace RaSetMaker.Tests.MatchersTests
{
    public class Md5MatcherTests : MatcherTestBase
    {
        [Theory]
        [InlineData("../../../TestRoms/atari2600.zip", "0db4f4150fecf77e4ce72ca4d04c052f")]
        public void RomMatchesHash(string filePath, string expectedHash)
        {
            var sys = GetGameSystemByName("Atari 2600");
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var foundRom = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);

        }

        [Theory]
        [InlineData("../../../TestRoms/atari2600.zip", "0db4f4150fecf77e4ce72ca4d04c052f")]
        public void RomExtensionDoesNotMatchSystem(string filePath, string expectedHash)
        {
            var sys = GetGameSystemByName("Atari 2600");
            sys.SupportedExtensions = [".rom"]; // This should not match
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var foundRom = matcher.FindRom(new FileInfo(filePath));

            Assert.Null(foundRom);
        }
    }
}
