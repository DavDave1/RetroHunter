using DiskReader.Chd;

namespace DiskReader.IsoFS;

public class RawIsoFileSystemProvider : IFileSystemProvider
{
    public RawIsoFileSystemProvider(string filePath, DiskImage.ReadMode readMode)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();

        _reader = extension switch
        {
            ".cue" => new BinCueDataReader(filePath, readMode),
            ".chd" => new ChdDataReader(filePath, readMode),
            _ => throw new NotSupportedException($"Extension {extension} is not supported by OperaFSReader"),
        };

        _primaryVolumeInfo = ReadPrimaryVolumeInfo();
    }

    public List<string> GetAllTrackFiles() => _reader?.GetAllTrackFiles() ?? [];

    public byte[]? ReadFile(string filename)
    {
        var record = GetDirectory(filename);

        if (record == null)
        {
            return null;
        }

        _reader.Seek(record.Location);

        var buffer = new byte[record.DataLength];

        if (!_reader.Read(buffer))
        {
            return null;
        }

        return buffer;
    }

    public byte[] GetVolumeHeader()
    {
        _reader.SeekRelative(0);

        var volHeader = new byte[2048];
        _reader.Read(volHeader);

        return volHeader;
    }

    private DirectoryRecord? GetDirectory(string filename)
    {
        _reader.OpenFirstTrack();

        do
        {
            _primaryVolumeInfo = ReadPrimaryVolumeInfo();

            var currentDir = _primaryVolumeInfo.RootDirectory;

            foreach (var dir in filename.Split("/"))
            {
                if (currentDir != null)
                {
                    currentDir = GetChild(currentDir, dir);
                }
            }

            if (currentDir != null)
            {
                return currentDir;
            }
        }
        while (_reader.OpenNextTrack());

        return null;
    }

    public List<DirectoryRecord> GetChildren(DirectoryRecord parent)
    {
        _reader.Seek(parent.Location);

        var data = new byte[parent.DataLength];

        _reader.Read(data);

        int dataOffset = 0;
        List<DirectoryRecord> children = [];
        while (dataOffset < data.Length)
        {
            var record = new DirectoryRecord(data[dataOffset..]);

            if (record.RecordLength == 0)
            {
                break;
            }

            children.Add(record);

            dataOffset += record.RecordLength;
        }

        return children;
    }

    public DirectoryRecord? GetChild(DirectoryRecord parent, string childName)
    {
        _reader.Seek(parent.Location);

        var data = new byte[parent.DataLength];

        _reader.Read(data);

        int dataOffset = 0;
        while (dataOffset < data.Length)
        {
            var record = new DirectoryRecord(data[dataOffset..]);

            if (record.FileName == childName)
            {
                return record;
            }

            if (record.RecordLength == 0)
            {
                break;
            }

            dataOffset += record.RecordLength;

        }

        return null;
    }

    private PrimaryVolume ReadPrimaryVolumeInfo()
    {
        uint volumeInfoBlock = Constants.DESCRIPTORS_START_ADDR;

        var volumeInfoData = new byte[2048];

        _reader.SeekRelative(volumeInfoBlock);
        _reader.Read(volumeInfoData);

        if (volumeInfoData[0] == Constants.PRIMARY_VOLUME_ID &&
            volumeInfoData[1..6].SequenceEqual(Constants.STANDARD_ID))
        {
            return new(volumeInfoData);
        }

        throw new UnsupportedFormatException("No primary volume info found");
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

    public void Dispose()
    {
        _reader.Dispose();
    }

    private readonly IDiskDatakReader _reader;
    private PrimaryVolume _primaryVolumeInfo;

}
