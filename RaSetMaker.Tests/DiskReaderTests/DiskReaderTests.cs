
using DiskReader;

namespace RaSetMaker.Tests.DiskReaderTests
{

    public class DiskReaderTests
    {
        [Theory]
        [InlineData("../../../TestRoms/ps1/007 Racing (USA).cue")]
        public void ReadCueImage(string filePath)
        {
            Iso9660Image disk = new(filePath);

            var dataBuffer = disk.ReadFile("SYSTEM.CNF");
            Assert.NotNull(dataBuffer);
        }

        [Theory]
        [InlineData("../../../TestRoms/psp.iso")]
        public void ReadIsoImage(string filePath)
        {
            var exception = Record.Exception(() => new Iso9660Image(filePath));
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("../../../TestRoms/007 Racing (USA).chd")]
        public void ReadChdImage(string filePath)
        {
            var exception = Record.Exception(() => new Iso9660Image(filePath));
            Assert.Null(exception);
        }

    }
}
