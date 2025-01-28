

using DiscUtils.Iso9660;
using System.ComponentModel.DataAnnotations;

namespace DiskReader
{
    public class IsoReader : IDiskReader
    {
        public List<string> GetAllTrackFiles() => [_isoFile?.FullName];

        public bool Load(string filePath)
        {
            _isoStream = File.OpenRead(filePath);
            _cdReader = new CDReader(_isoStream, true);

            return true;
        }

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

        private FileInfo _isoFile;
        private FileStream _isoStream;
        private CDReader _cdReader;

}
}
