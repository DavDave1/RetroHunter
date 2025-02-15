namespace DiskReader
{
    public interface IDiskDatakReader
    {
        bool Load(string FilePath);

        bool Seek(uint lba);

        bool SeekRelative(uint lba);

        bool Read(byte[] buffer);

        List<string> GetAllTrackFiles();

        void OpenFirstTrack();

        bool OpenNextTrack();
    }
}
