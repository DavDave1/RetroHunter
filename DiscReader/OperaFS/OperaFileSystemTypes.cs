
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Unicode;

namespace DiskReader.OperaFS;

public class VolumeHeaderInfo
{
    public string Comment { get; private set; }
    public string Label { get; private set; }
    public uint BlockSize { get; private set; }
    public uint BlockCount { get; private set; }

    public uint RootDirectoryIdentifier { get; private set; }
    public uint RootDirectoryBlocksCount { get; private set; }
    public uint RootDirectoryBlockSize { get; private set; }
    public uint RootDirectoryCopiesCount { get; private set; }
    public uint RootDirectoryFirstCopyLocation { get; private set; }


    public VolumeHeaderInfo(byte[] data)
    {
        if (data.Length != 132)
        {
            throw new Exception("Invalid volume header size");
        }

        if (!data[0..VOLUME_HEADER_IDENTIFIER.Length].SequenceEqual(VOLUME_HEADER_IDENTIFIER))
        {
            throw new Exception("Invalid volume header identifier");
        }

        Comment = Encoding.UTF8.GetString(data.AsSpan()[8..40]).Replace("\0", "");
        Label = Encoding.UTF8.GetString(data.AsSpan()[40..72]).Replace("\0", "");
        BlockSize = BitConverter.ToUInt32(data[76..80].Reverse().ToArray());
        BlockCount = BitConverter.ToUInt32(data[80..84].Reverse().ToArray());
        RootDirectoryIdentifier = BitConverter.ToUInt32(data[84..88].Reverse().ToArray());
        RootDirectoryBlocksCount = BitConverter.ToUInt32(data[88..92].Reverse().ToArray());
        RootDirectoryBlockSize = BitConverter.ToUInt32(data[92..96].Reverse().ToArray());
        RootDirectoryCopiesCount = BitConverter.ToUInt32(data[96..100].Reverse().ToArray());
        RootDirectoryFirstCopyLocation = BitConverter.ToUInt32(data[100..104].Reverse().ToArray());

    }

    private static readonly byte[] VOLUME_HEADER_IDENTIFIER = [0x01, 0x5A, 0x5A, 0x5A, 0x5A, 0x5A, 0x01];
}

public class OperaFsDirectory
{
    public OperaFsDirectoryHeader Header { get; private set; }

    public List<OperaFsEntry> Entries { get; private set; } = [];

    public OperaFsDirectory(IDiskDatakReader reader, uint currentSector)
    {
        var headerBuffer = new byte[20];
        reader.Read(headerBuffer);

        Header = new(headerBuffer);

        while (true)
        {

            var entryBuffer = new byte[72];
            reader.Read(entryBuffer);

            var entry = new OperaFsEntry(entryBuffer);

            Entries.Add(entry);

            if (entry.IsLastEntry())
            {
                break;
            }

            var offset = entry.CopiesCount * 4;

            if (offset > 0)
            {
                var offsBuffer = new byte[offset];
                reader.Read(offsBuffer);
            }
        }

    }
}

public class OperaFsDirectoryHeader(byte[] data)
{
    public int NextBlock { get; private set; } = BitConverter.ToInt32(data[0..4].Reverse().ToArray());
    public int PreviousBlock { get; private set; } = BitConverter.ToInt32(data[4..8].Reverse().ToArray());
    public uint Flags { get; private set; } = BitConverter.ToUInt32(data[8..12].Reverse().ToArray());
    public uint DirectoryEndOffset { get; private set; } = BitConverter.ToUInt32(data[12..16].Reverse().ToArray());
    public uint DirectoryStartOffset { get; private set; } = BitConverter.ToUInt32(data[16..20].Reverse().ToArray());
}

public class OperaFsEntry(byte[] data)
{
    public uint Flags { get; private set; } = BitConverter.ToUInt32(data[0..4].Reverse().ToArray());
    public uint Identifier { get; private set; } = BitConverter.ToUInt32(data[4..8].Reverse().ToArray());
    public string EntryType { get; private set; } = Encoding.ASCII.GetString(data[8..12]);
    public uint BlockSize { get; private set; } = BitConverter.ToUInt32(data[12..16].Reverse().ToArray());
    public uint ByteCount { get; private set; } = BitConverter.ToUInt32(data[16..20].Reverse().ToArray());
    public uint BlockCount { get; private set; } = BitConverter.ToUInt32(data[20..24].Reverse().ToArray());
    public uint Burst { get; private set; } = BitConverter.ToUInt32(data[24..28].Reverse().ToArray());
    public uint Gap { get; private set; } = BitConverter.ToUInt32(data[28..32].Reverse().ToArray());
    public string FileName { get; private set; } = Encoding.ASCII.GetString(data[32..64]).Replace("\0", "");

    public uint CopiesCount { get; private set; } = BitConverter.ToUInt32(data[64..68].Reverse().ToArray());

    public uint Offset { get; private set; } = BitConverter.ToUInt32(data[68..72].Reverse().ToArray());

    public bool IsFile() => (Flags & 0x2) == 0x2;

    public bool IsDir() => (Flags & 0x7) == 0x7;

    public bool IsLastEntry() => (Flags & 0x40000000) != 0;

    public bool IsLastEntryInBlock() => (Flags & 0x80000000) != 0;


    public OperaFsDirectory AsDirectory(IDiskDatakReader reader)
    {
        if (!IsDir())
        {
            throw new InvalidOperationException("Entry is not a directory");
        }

        reader.Seek(Offset);

        return new(reader, Offset);
    }
}
