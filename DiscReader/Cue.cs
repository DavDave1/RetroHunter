
using System.Text.RegularExpressions;

namespace DiskReader
{
    public partial class Cue
    {

        public List<Track> Tracks { get; private set; } = [];
    
        public bool Parse(Stream cueFileStream)
        {
            using TextReader reader = new StreamReader(cueFileStream);

            var currLineType = LineType.File;
            int lineNr = 0;
            var currTrack = new Track();
            var line = reader.ReadLine()?.TrimStart();
            while (line != null)
            {
                if (!line.StartsWith(LINE_START[currLineType]))
                {
                    throw new Exception($"Unexpected tag at line {lineNr}");
                }

                if (currLineType == LineType.File)
                {
                    currTrack = new Track
                    {
                        FilePath = line.Split('\"').Skip(1).First()
                    };

                    line = reader.ReadLine()?.TrimStart();
                    currLineType = LineType.Track;
                }
                else if (currLineType == LineType.Track)
                {
                    var lineSplit = line.Split(' ').ToList();
                    currTrack.TrackNr = int.Parse(lineSplit[1]);

                    if (lineSplit[2] == "AUDIO")
                    {
                        currTrack.TrackType = Track.ETrackType.Audio;
                    }
                    else if (lineSplit[2].StartsWith("MODE2"))
                    {
                        currTrack.TrackType = Track.ETrackType.Mode2;
                        currTrack.SectorSize = int.Parse(lineSplit[2].Split('/').Skip(1).First());
                    }
                    else
                    {
                        throw new Exception($"Unknown track type on line {lineNr}");
                    }

                    line = reader.ReadLine()?.TrimStart();
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

                    currTrack.Indices.Add(index);

                    line = reader.ReadLine()?.TrimStart();
                    if (line != null && line.StartsWith(LINE_START[LineType.File]))
                    {
                        Tracks.Add(currTrack);
                        currLineType = LineType.File;
                    }
                    else if (line == null)
                    {
                        Tracks.Add(currTrack);
                    }
                }
            }

            return Tracks.Any(t => t.TrackType == Track.ETrackType.Mode2);
        }

        private enum LineType
        {
            File,
            Track,
            Index
        }

        private static readonly Dictionary<LineType, string> LINE_START = new()
        {
            { LineType.File, "FILE" },
            { LineType.Track, "TRACK" },
            { LineType.Index, "INDEX" },
        };



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
                Mode2,
                Audio
            }

            public string FilePath { get; set; }
            public int TrackNr { get; set; }

            public int SectorSize { get; set; }

            public int SectorHeaderSize =>
                TrackType switch
                {
                    ETrackType.Mode2 => 24,
                    _ => throw new Exception("Unknown track type")
                };
            
            public ETrackType TrackType { get; set; }
            public List<TrackIndex> Indices { get; set; } = [];
        }

        [GeneratedRegex("\"(.*)\"")]
        private static partial Regex FilePathPattern();
    }
}
