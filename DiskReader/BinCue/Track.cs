
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

    public uint TrackSectorBegin => (uint)Indices.Sum(i => i.SectorsOffset);

    public uint TrackFileOffset => HighDensityTrack ? 45000u : 0;

    public int SectorHeaderSize =>
        TrackType switch
        {
            ETrackType.Mode1 or ETrackType.Mode1Raw => 16,
            ETrackType.Mode2 or ETrackType.Mode2Raw => 24,
            _ => throw new Exception("Unknown track header size")
        };

    public ETrackType TrackType { get; set; }
    public List<TrackIndex> Indices { get; set; } = [];

    public bool HighDensityTrack { get; set; } = false;
}

