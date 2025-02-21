

using DiscUtils.Iso9660;

namespace DiskReader.IsoFS
{
    public class IsoImageFileSystemProvider : IFileSystemProvider
    {
        public IsoImageFileSystemProvider(string filePath)
        {
            _isoFileName = filePath;
            _isoStream = File.OpenRead(_isoFileName);
            _cdReader = new CDReader(_isoStream, true);

        }
        public List<string> GetAllTrackFiles() => [_isoFileName];

        public byte[]? ReadFile(string fileName)
        {
            try
            {
                var fileStream = _cdReader.OpenFile(fileName, FileMode.Open);

                var buffer = new byte[fileStream.Length];
                fileStream.ReadExactly(buffer);
                return buffer;
            }
            catch (FileNotFoundException)
            {
            }

            return null;
        }

        public byte[] GetVolumeHeader()
        {
            throw new NotImplementedException();
        }

        public bool ReadDataRaw(byte[] buffer, uint sector, uint track, uint session)
        {
            throw new NotImplementedException();
        }

        private string _isoFileName;
        private readonly FileStream _isoStream;
        private readonly CDReader _cdReader;

    }
}
