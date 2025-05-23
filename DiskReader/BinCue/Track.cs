
namespace DiskReader.BinCue;

public class Track
{
    public class TrackIndex
    {
        public uint nr;
        public uint mm;
        public uint ss;
        public uint ff;

        public uint SectorsOffset => (mm * 60) + ss * 75 + ff;
    }

    public enum ETrackType
    {
        Mode1,
        Mode1Raw,
        Mode2,
        Mode2Raw,
        Mode2Form1,
        Mode2Form2,
        Mode2FormMix,
        Audio
    }

    public string FilePath { get; set; } = "";

    public int TrackNr { get; set; }

    public int SectorSize { get; set; }

    public int TrackSize { get; set; }

    public uint SessionNr { get; set; }

    public uint PregapSectors => (uint)Indices.Sum(i => i.SectorsOffset);

    public uint FileSectorOffset { get; set; }

    public int SectorHeaderSize =>
        TrackType switch
        {
            ETrackType.Mode1 or ETrackType.Mode1Raw => 16,
            ETrackType.Mode2 or ETrackType.Mode2Raw => 24,
            ETrackType.Audio => 0,
            _ => throw new Exception("Unknown track header size")
        };

    public int SectorRawSize =>
        TrackType switch
        {
            ETrackType.Audio => SectorSize,
            _ => 2048,
        };

    public ETrackType TrackType { get; set; }
    public List<TrackIndex> Indices { get; set; } = [];
}

