using RaSetMaker.Models;
using RaSetMaker.Utils;
using RaSetMaker.Utils.Matchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaSetMaker.Tests.MatchersTests
{
    public class N64MatcherTests : MatcherTestBase
    {
        [Theory]
        [InlineData("../../../TestRoms/nintendo64_bigendian.zip", "755df7f57edf87706d4c80ff15883312")]
        public void BigEndianRomMatchesHash(string filePath, string expectedHash)
        {
            var n64 = GetGameSystemByName("Nintendo 64");
            AddRomWithHash(n64, expectedHash);

            var matcher = RomMatcherFactory.Create(n64);
            Assert.NotNull(matcher);
            var (foundRom, _) = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);
        }

        [Theory]
        [InlineData("../../../TestRoms/nintendo64_byteswapped.zip", "755df7f57edf87706d4c80ff15883312")]
        public void ByteSwappedRomMatchesHash(string filePath, string expectedHash)
        {
            var n64 = GetGameSystemByName("Nintendo 64");
            AddRomWithHash(n64, expectedHash);

            var matcher = RomMatcherFactory.Create(n64);
            Assert.NotNull(matcher);
            var (foundRom, _) = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);
        }
    }
}
