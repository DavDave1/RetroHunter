
using System.Runtime.CompilerServices;

namespace DiskReader
{
    public class Iso9660Image
    {
        public Iso9660Image(string filePath)
        {
            if (Path.GetExtension(filePath) == ".iso")
            {
                _fsProvider = new IsoImageFileSystemProvider(filePath);
            }
            else
            {
                try
                {
                    _fsProvider = new RawIsoFileSystemProvider(filePath);
                }
                catch (UnsupportedFormatException)
                {
                    _fsProvider = new OperaFS.OperaFileSystemProvider(filePath);
                }
            }
        }

        public List<string> GetAllTrackFiles()
        {
            return _fsProvider?.GetAllTrackFiles() ?? [];
        }

        public byte[]? ReadFile(string filename)
        {
            return _fsProvider?.ReadFile(filename);
        }

        public byte[]? GetVolumeHeader()
        {
            return _fsProvider?.GetVolumeHeader();
        }

        private IFileSystemProvider? _fsProvider;
    }
}
