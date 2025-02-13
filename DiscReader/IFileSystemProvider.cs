namespace DiskReader
{
    public interface IFileSystemProvider
    {
        bool Load(string filePath);

        List<string> GetAllTrackFiles();

        byte[]? ReadFile(string fileName);

        byte[]? GetVolumeHeader();
    }
}
