using RaSetMaker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaSetMaker.Tests.MatchersTests
{
    public class NdsMatcherTests : MatcherTestBase
    {
        [Theory]
        [InlineData("../../../TestRoms/nds.zip", "e6d6d2daad4cc49483793ba298067065")]
        public void RomMatchesHash(string filePath, string expectedHash)
        {
            var sys = GetGameSystemByName("Nintendo DS");
            AddRomWithHash(sys, expectedHash);

            var matcher = RomMatcherFactory.Create(sys);

            var (foundRom, _) = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);
        }
    }
}
