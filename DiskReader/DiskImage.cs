
using DiskReader.IsoFS;

namespace DiskReader
{
    public class DiskImage
    {
        public DiskImage(string filePath)
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
