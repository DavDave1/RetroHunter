namespace DiskReader
{
    public interface IDiskReader
    {
        bool Load(string filePath);

        List<string> GetAllTrackFiles();

        byte[]? ReadFile(string fileName);
    }
}
