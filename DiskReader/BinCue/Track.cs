
namespace DiskReader.BinCue;

public class Track
{
    public class TrackIndex
    {
        public int nr;
        public int hh;
        public int mm;
        public int ss;
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

    public int TrackSectorBegin { get; set; }

    public int SectorHeaderSize =>
        TrackType switch
        {
            ETrackType.Mode1 or ETrackType.Mode1Raw => 16,
            ETrackType.Mode2 or ETrackType.Mode2Raw => 24,
            _ => throw new Exception("Unknown track header size")
        };

    public ETrackType TrackType { get; set; }
    public List<TrackIndex> Indices { get; set; } = [];
}

