
using System.Runtime.CompilerServices;

namespace DiskReader
{
    public class Iso9660Image
    {
        public bool Load(string FilePath)
        {
            if (Path.GetExtension(FilePath) == ".iso")
            {
                _fsProvider = new IsoImageFileSystemProvider();
            }
            else
            {
                _fsProvider = new RawFileSystemProvider();
            }

            if (!_fsProvider.Load(FilePath))
            {
                return false;
            }
            

            return true;
        }

        public List<string> GetAllTrackFiles()
        {
            return _fsProvider?.GetAllTrackFiles() ?? [];
        }

        public byte[]? ReadFile(string filename)
        {
            return _fsProvider?.ReadFile(filename);
        }


        private IFileSystemProvider? _fsProvider;
    }
}
