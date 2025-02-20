using DiskReader.Chd;

namespace DiskReader;

public class RawFileSystemProvider : IFileSystemProvider
{
    public RawFileSystemProvider(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();

        _reader = extension switch
        {
            ".cue" => new BinCueDataReader(filePath),
            ".chd" => new ChdDataReader(filePath),
            _ => throw new NotSupportedException($"Extension {extension} is not supported by OperaFSReader"),
        };

        _reader.OpenFirstTrack();
    }

    public List<string> GetAllTrackFiles() => _reader?.GetAllTrackFiles() ?? [];

    public byte[] GetVolumeHeader()
    {
        _reader.SeekRelative(0);

        var volHeader = new byte[2048];
        _reader.Read(volHeader);

        return volHeader;
    }
    public bool ReadDataRaw(byte[] buffer, uint track, uint sector)
    {
        _reader.OpenFirstTrack();

        var currTrack = 0;
        while (currTrack < track)
        {
            if (_reader.OpenNextTrack())
            {
                currTrack++;
            }
            else
            {
                return false;
            }
        }

        return _reader.SeekRelative(sector) && _reader.Read(buffer);
    }

    public byte[]? ReadFile(string fileName)
    {
        throw new NotImplementedException();
    }

    private readonly IDiskDatakReader _reader;
}
