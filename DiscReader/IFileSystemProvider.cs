namespace DiskReader
{
    public interface IFileSystemProvider
    {
        List<string> GetAllTrackFiles();

        byte[]? ReadFile(string fileName);

        byte[] GetVolumeHeader();
    }
}
