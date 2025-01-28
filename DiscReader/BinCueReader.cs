namespace DiskReader;

public class BinCueReader : IDiskReader
{
    public bool Load(string FilePath)
    {
        _cueFile = new FileInfo(FilePath);          
        using var cueStream = _cueFile.OpenRead();

        if (cueStream == null)
        {
            return false;
        }

        bool cueOK = _cue.Parse(cueStream);

        if (!cueOK) 
        { 
            return false; 
        }

        _track = GetBin(_cue.Tracks[_trackIndex].FilePath);
        _trackSize = (int)GetBinLength(_cue.Tracks[_trackIndex].FilePath);

        if (_track == null)
        {
            return false;
        }

        _sectorSize = _cue.Tracks[_trackIndex].SectorSize;
        _sectorHeaderSize = _cue.Tracks[_trackIndex].SectorHeaderSize;

        ReadPrimaryVolumeInfo();

        return true;
    }

    public List<string> GetAllTrackFiles()
    {
        return [_cueFile?.FullName, .. _cue.Tracks.Select(t => GetBinAbsolutePath(t.FilePath))];
    }

    public byte[]? ReadFile(string filename)
    {
        var record = GetDirectory(filename);

        if (record == null)
        {
            return null;
        }

        Seek(record.Location);

        var buffer = new byte[record.DataLength];

        if (!Read(buffer))
        {
            return null;
        }

        return buffer;
    }

    private bool Seek(long lba)
    {
        if (_track == null)
        {
            return false;
        }

        // TODO: seek to next track. Is this even necessary for the purpose of 
        // computing MD5 hash? For PS1 games, it looks only first track contains data.
        if (_currentPosition >= _trackSize)
        {
            return false;
        }

        if (lba == _currentLba)
        {
            return true;
        }

        _currentLba = (int)lba;
        _currentPosition = (lba * _sectorSize) + _sectorHeaderSize;
        _track.Seek(_currentPosition, SeekOrigin.Begin);

        return true;
    }

    private bool Read(byte[] buffer)
    {
        if (_track == null)
        {
            return false;
        }

        int readLen = 0;
        int nextLba = _currentLba;
        while (readLen < buffer.Length)
        {
            var sizeToRead = Math.Min(2048, buffer.Length - readLen);

            _track.ReadExactly(buffer, readLen, sizeToRead);

            readLen += sizeToRead;
            nextLba++;
            if (!Seek(nextLba))
            {
                return false;
            }
        }

        return true;
    }


    private FileStream? GetBin(string fileName)
    {
        if (_cueFile == null)
        {
            return null;
        }

        return new FileInfo(GetBinAbsolutePath(fileName)).OpenRead();
    }

    private long GetBinLength(string fileName) => new FileInfo(GetBinAbsolutePath(fileName)).Length;

    private string GetBinAbsolutePath(string fileName) => $"{_cueFile?.Directory?.FullName}\\{fileName}";

    private void ReadPrimaryVolumeInfo()
    {
        bool volumeInfoBlockFound = false;
        int volumeInfoBlock = Constants.DESCRIPTORS_START_ADDR;

        var volumeInfoData = new byte[2048];

        while (!volumeInfoBlockFound)
        {
            if (!Seek(volumeInfoBlock))
            {
                return;
            }

            Read(volumeInfoData);

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
        Seek(parent.Location);

        var data = new byte[parent.DataLength];

        Read(data);

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
        Seek(parent.Location);

        var data = new byte[parent.DataLength];

        Read(data);

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


    private Stream? _track;
    private int _trackIndex;
    private int _trackSize;
    private int _sectorSize;
    private int _sectorHeaderSize;

    private FileInfo? _cueFile;
    private readonly Cue _cue = new();

    private long _currentPosition;
    private int _currentLba;

    private PrimaryVolume? _primaryVolumeInfo;
}
