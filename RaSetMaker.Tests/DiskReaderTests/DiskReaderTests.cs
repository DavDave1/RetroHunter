
using DiskReader;

namespace RaSetMaker.Tests.DiskReaderTests
{

    public class DiskReaderTests
    {
        [Theory]
        [InlineData("../../../TestRoms/ps1/007 Racing (USA).cue")]
        public void ReadCueImage(string filePath)
        {

            if (!Path.Exists(filePath))
            {
                return;
            }

            using DiskImage disk = new(filePath);

            var dataBuffer = disk.ReadFile("SYSTEM.CNF");
            Assert.NotNull(dataBuffer);
        }

        [Theory]
        [InlineData("../../../TestRoms/psp.iso")]
        public void ReadIsoImage(string filePath)
        {

            if (!Path.Exists(filePath))
            {
                return;
            }

            var exception = Record.Exception(() => new DiskImage(filePath));
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("../../../TestRoms/ps1/007 Racing (USA).chd")]
        public void ReadChdImage(string filePath)
        {

            if (!Path.Exists(filePath))
            {
                return;
            }

            var exception = Record.Exception(() => new DiskImage(filePath));
            Assert.Null(exception);
        }

    }
}
