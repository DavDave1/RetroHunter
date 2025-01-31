namespace DiskReader
{
    public interface IDiskDatakReader
    {
        bool Load(string FilePath);

        bool Seek(uint lba);

        bool Read(byte[] buffer);

        List<string> GetAllTrackFiles();
    }
}
