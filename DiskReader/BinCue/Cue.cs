
namespace DiskReader.BinCue;

public class Cue
{
    public List<Track> Tracks { get; private set; } = [];

    public Cue(string filePath, DiskImage.ReadMode readMode)
    {
        _cueFile = new FileInfo(filePath);
        _readMode = readMode;

        using var cueStream = _cueFile.OpenRead();
        using TextReader reader = new StreamReader(cueStream);

        var currLineType = LineType.File;
        int lineNr = 0;
        bool highDensityTrack = false;
        uint sessionNr = 1;

        while (true)
        {
            var line = reader.ReadLine()?.TrimStart();
            lineNr++;

            if (line == null)
            {
                break;
            }

            // ignore
            if (line.StartsWith(LINE_START[LineType.Catalog]) || line.StartsWith(LINE_START[LineType.Flags]))
            {
                continue;
            }

            // End of previous file, begin new
            if (currLineType == LineType.Index && !line.StartsWith(LINE_START[LineType.Index]))
            {
                currLineType = LineType.File;
            }

            if (line.StartsWith(LINE_START[LineType.Comment]))
            {
                var comment = line.Replace(LINE_START[LineType.Comment], "").Trim();

                // System specific hack to handle Dreamcast cue files.
                if (comment == "HIGH-DENSITY AREA")
                {
                    highDensityTrack = true;
                }

                if (comment.StartsWith("SESSION"))
                {
                    var sessionNrStr = comment.Replace("SESSION", "").Trim();
                    sessionNr = uint.Parse(sessionNrStr);
                }

                continue;
            }

            if (!line.StartsWith(LINE_START[currLineType]))
            {
                throw new DiskReaderException($"Unexpected tag at line {lineNr}");
            }

            if (currLineType == LineType.File)
            {
                // High density area starts at sector 45000.
                // See https://github.com/flyinghead/flycast/blob/e1da1c3a55ad09545d2eefbd8b909689602a4de7/core/imgread/cue.cpp#L123
                // TOOD: investigate why and possibly find a generic implementation for this.
                Tracks.Add(new Track
                {
                    FilePath = line.Split('\"').Skip(1).First(),
                    HighDensityTrack = highDensityTrack,
                    SessionNr = sessionNr
                });

                highDensityTrack = false;
                currLineType = LineType.Track;
            }
            else if (currLineType == LineType.Track)
            {
                var lineSplit = line.Split(' ').ToList();
                Tracks.Last().TrackNr = int.Parse(lineSplit[1]);

                if (lineSplit[2] == "AUDIO")
                {
                    Tracks.Last().TrackType = Track.ETrackType.Audio;
                    Tracks.Last().SectorSize = 2352;
                }
                else if (lineSplit[2].StartsWith("MODE2"))
                {
                    Tracks.Last().TrackType = Track.ETrackType.Mode2;
                    Tracks.Last().SectorSize = int.Parse(lineSplit[2].Split('/').Skip(1).First());
                }
                else if (lineSplit[2].StartsWith("MODE1"))
                {
                    Tracks.Last().TrackType = Track.ETrackType.Mode1;
                    Tracks.Last().SectorSize = int.Parse(lineSplit[2].Split('/').Skip(1).First());
                }
                else
                {
                    throw new DiskReaderException($"Unknown track type on line {lineNr}");
                }

                currLineType = LineType.Index;
            }
            else if (currLineType == LineType.Index)
            {
                var lineSplit = line.Split(' ').ToList();
                var indexNr = uint.Parse(lineSplit[1]);

                var timeSplit = lineSplit[2].Split(":").ToList();

                var index = new Track.TrackIndex
                {
                    nr = indexNr,
                    mm = uint.Parse(timeSplit[0]),
                    ss = uint.Parse(timeSplit[1]),
                    ff = uint.Parse(timeSplit[2])
                };

                Tracks.Last().Indices.Add(index);
            }
        }

        foreach (var track in Tracks)
        {
            track.TrackSize = (int)GetBinLength(track.FilePath);
        }
    }

    public bool SelectFirstTrack(uint session)
    {
        _trackIndex = Tracks
            .FindIndex(t => t.SessionNr == session && (_readMode == DiskImage.ReadMode.TreatAudioTracksAsData || t.TrackType != Track.ETrackType.Audio));

        return _trackIndex >= 0;
    }

    public bool SelectNextTrack(uint session)
    {
        _trackIndex++;

        while (_trackIndex < Tracks.Count)
        {
            if ((Tracks[_trackIndex].SessionNr == session) &&
                (_readMode == DiskImage.ReadMode.TreatAudioTracksAsData || Tracks[_trackIndex].TrackType != Track.ETrackType.Audio))
            {
                break;
            }

            _trackIndex++;
        }

        return _trackIndex < Tracks.Count;
    }

    public Stream OpenTrack()
    {
        return new FileInfo(GetBinAbsolutePath(Tracks[_trackIndex].FilePath)).OpenRead();
    }

    public Track CurrentTrack() => Tracks[_trackIndex];

    public FileStream? GetBin(string fileName)
    {
        return new FileInfo(GetBinAbsolutePath(fileName)).OpenRead();
    }

    public List<string> GetAllFiles()
    {
        return [_cueFile.FullName, .. Tracks.Select(t => GetBinAbsolutePath(t.FilePath))];
    }

    private long GetBinLength(string fileName) => new FileInfo(GetBinAbsolutePath(fileName)).Length;

    private string GetBinAbsolutePath(string fileName) => $"{_cueFile?.Directory?.FullName}/{fileName}";

    private enum LineType
    {
        File,
        Track,
        Index,
        Catalog,
        Comment,
        Flags
    }

    private static readonly Dictionary<LineType, string> LINE_START = new()
    {
        { LineType.File, "FILE" },
        { LineType.Track, "TRACK" },
        { LineType.Index, "INDEX" },
        { LineType.Catalog, "CATALOG" },
        { LineType.Comment, "REM" },
        { LineType.Flags, "FLAGS" }
    };

    private readonly FileInfo _cueFile;
    private readonly DiskImage.ReadMode _readMode;

    private int _trackIndex;
}
