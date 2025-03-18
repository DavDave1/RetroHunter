
using System.Text;
using System.IO.Hashing;

namespace RomPatcher;


/// <summary>
/// 
/// https://github.com/blakesmith/rombp/blob/master/docs/bps_spec.md
///
/// </summary>

public class Bps : IDisposable
{
    public Bps(string patchFile)
    {
        _bps = File.OpenRead(patchFile);
        _patchFile = patchFile;

        ParseHeader();
    }

    public uint SourceChecksum => _sourceChecksum;

    public uint TargetChecksum => _targetChecksum;

    public ulong TargetLength => _tragetSize;

    public bool IsValid() => ComputeChecksum() == _patchChecksum;

    public void BeginRead() => SeekBegin();

    public IAction Next()
    {
        ulong data = DecodeNumber();
        ulong commandID = data & 3;
        ulong length = (data >> 2) + 1;

        return commandID switch
        {
            0 => new SourceReadAction(length),
            1 => new TargetReadAction(ReadData(length)),
            2 => new SourceCopyAction(length, DecodeOffset()),
            3 => new TargetCopyAction(length, DecodeOffset()),
            _ => throw new ArgumentException("Invalid command id", _patchFile)
        };
    }

    public bool HasNext() => _bps.Position < _bps.Length - 12;

    public void Dispose()
    {
        _bps.Dispose();
    }

    private void ParseHeader()
    {
        var buffer = new byte[4];

        _bps.ReadExactly(buffer);

        string bpsId = Encoding.ASCII.GetString(buffer);

        if (bpsId != "BPS1")
            throw new ArgumentException("Not a valid BPS file", _patchFile);

        _sourceSize = DecodeNumber();
        _tragetSize = DecodeNumber();

        ulong metadataSize = DecodeNumber();

        var metadataBuffer = new byte[metadataSize];

        _bps.ReadExactly(metadataBuffer);

        _actionsStartPosition = _bps.Position;

        _metadata = Encoding.UTF8.GetString(metadataBuffer);

        _bps.Seek(-12, SeekOrigin.End);

        var u32buffer = new byte[4];

        _bps.ReadExactly(u32buffer);
        _sourceChecksum = BitConverter.ToUInt32(u32buffer);

        _bps.ReadExactly(u32buffer);
        _targetChecksum = BitConverter.ToUInt32(u32buffer);

        _bps.ReadExactly(u32buffer);
        _patchChecksum = BitConverter.ToUInt32(u32buffer);

        SeekBegin();
    }

    private uint ComputeChecksum()
    {
        _bps.Seek(0, SeekOrigin.Begin);

        var buffer = new byte[_bps.Length - 4];

        _bps.ReadExactly(buffer);

        SeekBegin();

        return Crc32.HashToUInt32(buffer);

    }

    private ulong DecodeNumber()
    {
        ulong data = 0, shift = 1;
        while (true)
        {
            ulong x = ReadByte();
            data += (x & 0x7f) * shift;
            if ((x & 0x80) != 0)
                break;

            shift <<= 7;
            data += shift;
        }
        return data;

    }

    private long DecodeOffset()
    {
        ulong data = DecodeNumber();

        long offset = (long)(data >> 1);

        if ((data & 1) != 0)
            offset = -offset;

        return offset;
    }

    private ulong ReadByte()
    {
        return (ulong)_bps.ReadByte();
    }

    private byte[] ReadData(ulong length)
    {
        var buffer = new byte[length];
        _bps.ReadExactly(buffer);
        return buffer;
    }

    private void SeekBegin() => _bps.Seek(_actionsStartPosition, SeekOrigin.Begin);

    private readonly string _patchFile = "";
    private readonly Stream _bps;

    private ulong _sourceSize;
    private ulong _tragetSize;

    private string _metadata = "";

    private uint _sourceChecksum;
    private uint _targetChecksum;
    private uint _patchChecksum;

    private long _actionsStartPosition;
}

public interface IAction
{
    void Apply(BpsStream src, BpsStream tgt);
}

public class BpsStream(Stream stream)
{
    public Stream stream = stream;
    public long relativeOffset;
}


public class SourceReadAction(ulong length) : IAction
{
    public void Apply(BpsStream src, BpsStream tgt)
    {
        var srcPos = src.stream.Position;
        src.stream.Seek(tgt.stream.Position, SeekOrigin.Begin);

        byte[] data = new byte[length];
        src.stream.ReadExactly(data);

        tgt.stream.Write(data);

        src.stream.Seek(srcPos, SeekOrigin.Begin);
    }
}

public class TargetReadAction(byte[] data) : IAction
{
    public void Apply(BpsStream src, BpsStream tgt)
    {
        tgt.stream.Write(data);
    }
}

public class SourceCopyAction(ulong length, long sourceOffset) : IAction
{
    public void Apply(BpsStream src, BpsStream tgt)
    {
        src.relativeOffset += sourceOffset;
        src.stream.Seek(src.relativeOffset, SeekOrigin.Begin);

        byte[] data = new byte[length];
        src.stream.ReadExactly(data);

        tgt.stream.Write(data);

        src.relativeOffset += (long)length;
    }
}
public class TargetCopyAction(ulong length, long targetOffset) : IAction
{
    public void Apply(BpsStream src, BpsStream tgt)
    {
        tgt.relativeOffset += targetOffset;

        long remaining = (long)length;
        while (remaining > 0)
        {
            var tgtPosition = tgt.stream.Position;
            tgt.stream.Seek(tgt.relativeOffset, SeekOrigin.Begin);

            var readLength = Math.Min(tgt.stream.Length - tgt.stream.Position, remaining);
            byte[] data = new byte[readLength];
            tgt.stream.ReadExactly(data);

            tgt.stream.Seek(tgtPosition, SeekOrigin.Begin);

            tgt.stream.Write(data);
            tgt.relativeOffset += readLength;

            remaining -= readLength;
        }
    }
}