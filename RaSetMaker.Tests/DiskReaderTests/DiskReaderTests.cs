
using DiskReader;

namespace RaSetMaker.Tests.DiskReaderTests
{
  
    public class DiskReaderTests
    {
        [Theory]
        [InlineData("../../../TestRoms/ps1/007 Racing (USA).cue")]
        public void ReadCueImage(string filePath)
        {
            Iso9660Image disk = new();

            Assert.True(disk.Load(filePath));

            var filename = "SYSTEM.CNF";

            var dataBuffer = disk.ReadFile(filename);
            Assert.NotNull(dataBuffer);

        }

        [Theory]
        [InlineData("../../../TestRoms/psp.iso")]
        public void ReadIsoImage(string filePath)
        {
            Iso9660Image disk = new();
            Assert.True(disk.Load(filePath));
        }

        [Theory]
        [InlineData("../../../TestRoms/007 Racing (USA).chd")]
        public void ReadChdImage(string filePath)
        {
            Iso9660Image disk = new();
            Assert.True(disk.Load(filePath));
        }

    }
}
