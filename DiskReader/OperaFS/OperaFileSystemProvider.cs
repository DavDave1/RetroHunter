using System.Runtime.CompilerServices;
using DiskReader.Chd;

namespace DiskReader.OperaFS;

public class OperaFileSystemProvider : IFileSystemProvider
{
    public OperaFileSystemProvider(string filePath, DiskImage.ReadMode readMode)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();

        _reader = extension switch
        {
            ".cue" => new BinCueDataReader(filePath, readMode),
            ".chd" => new ChdDataReader(filePath, readMode),
            _ => throw new NotSupportedException($"Extension {extension} is not supported by OperaFSReader"),
        };

        var volHeaderBuffer = GetVolumeHeader();

        _volumeHeader = new(volHeaderBuffer);
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

    public byte[] GetVolumeHeader()
    {
        _reader.Seek(0);

        var volHeader = new byte[132];
        _reader.Read(volHeader);

        return volHeader;
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

    public bool ReadDataRaw(byte[] buffer, uint sector, uint track, uint session = 1)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _reader.Dispose();
    }

    private IDiskDatakReader _reader;
    private VolumeHeaderInfo? _volumeHeader;
}
