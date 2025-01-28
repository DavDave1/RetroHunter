
using System.Runtime.CompilerServices;

namespace DiskReader
{
    public class Iso9660Image
    {
        public bool Load(string FilePath)
        {
            // TODO: support other image formats
            if (Path.GetExtension(FilePath) == ".cue")
            {

                _diskReader = new BinCueReader();
            }
            else if (Path.GetExtension(FilePath) == ".iso")
            {
                _diskReader = new IsoReader();
            }
            else
            {
                return false;
            }

            if (!_diskReader.Load(FilePath))
            {
                return false;
            }
            

            return true;
        }

        public List<string> GetAllTrackFiles()
        {
            return _diskReader?.GetAllTrackFiles() ?? [];
        }

        public byte[]? ReadFile(string filename)
        {
            return _diskReader?.ReadFile(filename);
        }


        private IDiskReader? _diskReader;
    }
}
