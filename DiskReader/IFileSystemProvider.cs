namespace DiskReader
{
    public interface IFileSystemProvider : IDisposable
    {
        List<string> GetAllTrackFiles();

        byte[]? ReadFile(string fileName);

        byte[] GetVolumeHeader();

        bool ReadDataRaw(byte[] buffer, uint sector, uint track, uint session = 1);
    }
}
