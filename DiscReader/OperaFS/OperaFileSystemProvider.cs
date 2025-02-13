using System.Runtime.CompilerServices;

namespace DiskReader.OperaFS;

public class OperaFileSystemProvider : IFileSystemProvider
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

        return ReadVolumeHeaderInfo();
    }

    public List<string> GetAllTrackFiles() => _reader?.GetAllTrackFiles() ?? [];

    public byte[]? ReadFile(string filename)
    {
        var record = GetFile(filename);

        if (record == null)
        {
            return null;
        }

        _reader.Seek(record.Offset);

        var buffer = new byte[record.ByteCount];

        if (!_reader.Read(buffer))
        {
            return null;
        }

        return buffer;
    }

    public byte[]? GetVolumeHeader()
    {
        _reader.Seek(0);

        var volHeader = new byte[132];
        _reader.Read(volHeader);

        return volHeader;
    }

    private bool ReadVolumeHeaderInfo()
    {
        var volHeaderBuffer = GetVolumeHeader();

        _volumeHeader = new(volHeaderBuffer);

        return true;

    }

    private OperaFsEntry? GetFile(string filename)
    {
        if (_reader == null || _volumeHeader == null)
        {
            return null;
        }

        _reader.Seek(_volumeHeader.RootDirectoryFirstCopyLocation);

        var currentDir = new OperaFsDirectory(_reader, _volumeHeader.RootDirectoryFirstCopyLocation);

        OperaFsEntry? currentEntry = null;
        foreach (var dir in filename.Split("/"))
        {
            currentEntry = currentDir.Entries.FirstOrDefault(e => e.FileName == dir);

            if (currentEntry == null)
            {
                break;
            }

            if (currentEntry.IsDir())
            {
                currentDir = currentEntry.AsDirectory(_reader);
            }
        }

        return currentEntry;
    }

    private IDiskDatakReader? _reader;
    private VolumeHeaderInfo? _volumeHeader;
}
