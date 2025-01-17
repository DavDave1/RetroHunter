using RaSetMaker.Utils;

namespace RaSetMaker.Tests.MatchersTests
{
    public class FileNameHashMatcherTest : MatcherTestBase
    {
        [Theory]
        [InlineData("../../../TestRoms/progolf.zip", "d0f2f686b61f08f07cd2925bb3ae8b41")]
        public void RomMatchesHash(string filePath, string expectedHash)
        {
            var arcade = GetGameSystemByName("Arcade");
            AddRomWithHash(arcade, expectedHash);

            var matcher = RomMatcherFactory.Create(arcade);
            Assert.NotNull(matcher);
            var foundRom = matcher.FindRom(new FileInfo(filePath));

            Assert.NotNull(foundRom);
            Assert.Equal(expectedHash, foundRom.Hash);
        }

    }
}
