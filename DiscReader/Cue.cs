
using System.Text.RegularExpressions;

namespace DiskReader
{
    public class Cue
    {
        public List<Track> Tracks { get; private set; } = [];

        public bool Parse(Stream cueFileStream)
        {
            using TextReader reader = new StreamReader(cueFileStream);

            var currLineType = LineType.File;
            int lineNr = 0;
            bool highDensityTrack = false;

            while (true)
            {
                var line = reader.ReadLine()?.TrimStart();
                lineNr++;

                if (line == null)
                {
                    break;
                }

                // ignore
                if (line.StartsWith(LINE_START[LineType.Catalog]))
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

                    continue;
                }

                if (!line.StartsWith(LINE_START[currLineType]))
                {
                    throw new Exception($"Unexpected tag at line {lineNr}");
                }

                if (currLineType == LineType.File)
                {
                    // High density area starts at sector 45000.
                    // See https://github.com/flyinghead/flycast/blob/e1da1c3a55ad09545d2eefbd8b909689602a4de7/core/imgread/cue.cpp#L123
                    // TOOD: investigate why and possibly find a generic implementation for this.
                    Tracks.Add(new Track
                    {
                        FilePath = line.Split('\"').Skip(1).First(),
                        TrackSectorBegin = highDensityTrack ? 45000 : 0,
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
                        throw new Exception($"Unknown track type on line {lineNr}");
                    }

                    currLineType = LineType.Index;
                }
                else if (currLineType == LineType.Index)
                {
                    var lineSplit = line.Split(' ').ToList();
                    var indexNr = int.Parse(lineSplit[1]);

                    var timeSplit = lineSplit[2].Split(":").ToList();

                    var index = new Track.TrackIndex
                    {
                        nr = indexNr,
                        hh = int.Parse(timeSplit[0]),
                        mm = int.Parse(timeSplit[1]),
                        ss = int.Parse(timeSplit[2])
                    };

                    Tracks.Last().Indices.Add(index);
                }
            }

            return Tracks.Count > 0;
        }

        private enum LineType
        {
            File,
            Track,
            Index,
            Catalog,
            Comment

        }

        private static readonly Dictionary<LineType, string> LINE_START = new()
        {
            { LineType.File, "FILE" },
            { LineType.Track, "TRACK" },
            { LineType.Index, "INDEX" },
            { LineType.Catalog, "CATALOG" },
            { LineType.Comment, "REM" }
        };
    }
}
