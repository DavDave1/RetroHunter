using DiskReader.Chd;

namespace DiskReader;

public class RawFileSystemProvider : IFileSystemProvider
{
    public RawFileSystemProvider(string filePath, DiskImage.ReadMode readMode)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();

        _reader = extension switch
        {
            ".cue" => new BinCueDataReader(filePath, readMode),
            ".chd" => new ChdDataReader(filePath, readMode),
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
    public bool ReadDataRaw(byte[] buffer, uint sector, uint track, uint session = 1)
    {
        _reader.OpenFirstTrack(session);

        var currTrack = 0;
        while (currTrack < track)
        {
            if (_reader.OpenNextTrack(session))
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

    public void Dispose()
    {
        _reader.Dispose();
    }

    private readonly IDiskDatakReader _reader;
}
