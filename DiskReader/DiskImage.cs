
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

                var fsProvider = CreateProvider(filePath, ProviderType.RawIso);

                if (fsProvider == null)
                {
                    fsProvider = CreateProvider(filePath, ProviderType.Opera);
                }

                if (fsProvider == null)
                {
                    fsProvider = CreateProvider(filePath, ProviderType.Raw);
                }

                if (fsProvider == null)
                {
                    throw new UnsupportedFormatException($"Unknown filesystem for {filePath}");
                }

                _fsProvider = fsProvider;
            }
        }

        public List<string> GetAllTrackFiles()
        {
            return _fsProvider.GetAllTrackFiles() ?? [];
        }

        public byte[]? ReadFile(string filename)
        {
            return _fsProvider.ReadFile(filename);
        }

        public byte[]? GetVolumeHeader()
        {
            return _fsProvider.GetVolumeHeader();
        }

        public bool ReadDataRaw(byte[] buffer, uint track, uint sector)
        {
            return _fsProvider.ReadDataRaw(buffer, track, sector);
        }


        private enum ProviderType
        {
            RawIso,
            Opera,
            Raw
        }
        private static IFileSystemProvider? CreateProvider(string filePath, ProviderType type)
        {
            try
            {
                if (type == ProviderType.RawIso)
                {
                    return new RawIsoFileSystemProvider(filePath);
                }
                else if (type == ProviderType.Opera)
                {
                    return new OperaFS.OperaFileSystemProvider(filePath);
                }
                else
                {
                    return new RawFileSystemProvider(filePath);
                }
            }
            catch (UnsupportedFormatException)
            {
            }

            return null;
        }

        private readonly IFileSystemProvider _fsProvider;
    }
}
