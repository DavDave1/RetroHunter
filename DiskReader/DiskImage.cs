
using DiskReader.IsoFS;

namespace DiskReader
{
    public class DiskImage : IDisposable
    {
        public enum ReadMode
        {
            SkipAudioTracks,
            TreatAudioTracksAsData
        }

        public DiskImage(string filePath, ReadMode readMode = ReadMode.SkipAudioTracks)
        {
            if (Path.GetExtension(filePath) == ".iso")
            {
                _fsProvider = new IsoImageFileSystemProvider(filePath);
            }
            else
            {

                var fsProvider = CreateProvider(filePath, readMode, ProviderType.RawIso);

                if (fsProvider == null)
                {
                    fsProvider = CreateProvider(filePath, readMode, ProviderType.Opera);
                }

                if (fsProvider == null)
                {
                    fsProvider = CreateProvider(filePath, readMode, ProviderType.Raw);
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

        public bool ReadDataRaw(byte[] buffer, uint sector, uint track, uint session = 1)
        {
            return _fsProvider.ReadDataRaw(buffer, sector, track, session);
        }


        private enum ProviderType
        {
            RawIso,
            Opera,
            Raw
        }
        private static IFileSystemProvider? CreateProvider(string filePath, ReadMode mode, ProviderType type)
        {
            try
            {
                if (type == ProviderType.RawIso)
                {
                    return new RawIsoFileSystemProvider(filePath, mode);
                }
                else if (type == ProviderType.Opera)
                {
                    return new OperaFS.OperaFileSystemProvider(filePath, mode);
                }
                else
                {
                    return new RawFileSystemProvider(filePath, mode);
                }
            }
            catch (UnsupportedFormatException)
            {
            }

            return null;
        }

        public void Dispose()
        {
            _fsProvider.Dispose();
        }

        private readonly IFileSystemProvider _fsProvider;
    }
}
