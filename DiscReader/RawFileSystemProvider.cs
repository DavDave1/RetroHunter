namespace DiskReader;

public class RawFileSystemProvider : IFileSystemProvider
{
    public bool Load(string FilePath)
    {
        if (Path.GetExtension(FilePath) == ".cue")
        {
            _reader = new BinCueDataReader();
            
        }
        else if (Path.GetExtension(FilePath) == ".chd")
        {
            _reader = new ChdDataReader();
        }
        else
        {
            return false;
        }

        if (!_reader.Load(FilePath))
        {
            return false;
        }
        
        ReadPrimaryVolumeInfo();

        return true;
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

    private void ReadPrimaryVolumeInfo()
    {
        bool volumeInfoBlockFound = false;
        uint volumeInfoBlock = Constants.DESCRIPTORS_START_ADDR;

        var volumeInfoData = new byte[2048];

        while (!volumeInfoBlockFound)
        {
            if (!_reader.Seek(volumeInfoBlock))
            {
                return;
            }

            _reader.Read(volumeInfoData);

            if (volumeInfoData[0] == Constants.PRIMARY_VOLUME_ID &&
                volumeInfoData[1..6].SequenceEqual(Constants.STANDARD_ID))
            {
                volumeInfoBlockFound = true;
            }

            volumeInfoBlock++;
        }

        _primaryVolumeInfo = new(volumeInfoData);
    }

    private DirectoryRecord? GetDirectory(string filename)
    {
        var dirs = filename.Split("/");

        var currentDir = _primaryVolumeInfo?.RootDirectory;

        foreach (var dir in filename.Split("/"))
        {
            if (currentDir != null)
            {
                currentDir = GetChild(currentDir, dir);
            }
        }

        return currentDir;
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


    private IDiskDatakReader? _reader;
    private PrimaryVolume? _primaryVolumeInfo;
}
