using System.Xml;

namespace DiskReader
{
    public interface IDiskDatakReader
    {
        bool Seek(uint lba);

        bool SeekRelative(uint lba);

        bool Read(byte[] buffer);

        List<string> GetAllTrackFiles();

        bool OpenFirstTrack(uint session = 1);

        bool OpenNextTrack(uint session = 1);
    }
}
