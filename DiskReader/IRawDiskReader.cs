namespace DiskReader
{
    public interface IDiskDatakReader
    {
        bool Seek(uint lba);

        bool SeekRelative(uint lba);

        bool Read(byte[] buffer);

        List<string> GetAllTrackFiles();

        bool OpenFirstTrack();

        bool OpenNextTrack();
    }
}
