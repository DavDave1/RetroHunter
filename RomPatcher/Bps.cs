
using System.ComponentModel.DataAnnotations;
using System.IO.Hashing;
using System.Text;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RomPatcher;


/// <summary>
/// 
/// https://github.com/blakesmith/rombp/blob/master/docs/bps_spec.md
///
/// </summary>

public class Bps : IDisposable
{
    public static async Task<Bps> Create(string patchFile)
    {
        var bps = new Bps(patchFile);

        await bps.ParseHeader();

        return bps;
    }

    public uint SourceChecksum => _sourceChecksum;

    public uint TargetChecksum => _targetChecksum;

    public ulong TargetLength => _tragetSize;

    public bool IsValid() => ComputeChecksum() == _patchChecksum;

    public void BeginRead() => SeekBegin();

    public async Task<IAction> Next()
    {
        ulong data = DecodeNumber();
        ulong commandID = data & 3;
        ulong length = (data >> 2) + 1;

        return commandID switch
        {
            0 => new SourceReadAction(length),
            1 => new TargetReadAction(await ReadData(length)),
            2 => new SourceCopyAction(length, DecodeOffset()),
            3 => new TargetCopyAction(length, DecodeOffset()),
            _ => throw new ArgumentException("Invalid command id", _patchFile)
        };
    }

    public bool HasNext() => _bps.Position < _bps.Length - 12;

    public void Reset() => _bps.Seek(_actionsStartPosition, SeekOrigin.Begin);

    public void Dispose()
    {
        _bps.Dispose();
    }

    private Bps(string patchFile)
    {
        _bps = File.OpenRead(patchFile);
        _patchFile = patchFile;
    }

    private async Task ParseHeader()
    {
        var buffer = new byte[4];

        await _bps.ReadExactlyAsync(buffer);

        string bpsId = Encoding.ASCII.GetString(buffer);

        if (bpsId != "BPS1")
            throw new ArgumentException("Not a valid BPS file", _patchFile);

        _sourceSize = DecodeNumber();
        _tragetSize = DecodeNumber();

        ulong metadataSize = DecodeNumber();

        var metadataBuffer = new byte[metadataSize];

        await _bps.ReadExactlyAsync(metadataBuffer);

        _actionsStartPosition = _bps.Position;

        _metadata = Encoding.UTF8.GetString(metadataBuffer);

        _bps.Seek(-12, SeekOrigin.End);

        var u32buffer = new byte[4];

        await _bps.ReadExactlyAsync(u32buffer);
        _sourceChecksum = BitConverter.ToUInt32(u32buffer);

        await _bps.ReadExactlyAsync(u32buffer);
        _targetChecksum = BitConverter.ToUInt32(u32buffer);

        await _bps.ReadExactlyAsync(u32buffer);
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

    private async Task<byte[]> ReadData(ulong length)
    {
        var buffer = new byte[length];
        await _bps.ReadExactlyAsync(buffer);
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
    Task Apply(BpsStream src, BpsStream tgt);
}

public class BpsStream(Stream stream)
{
    public Stream stream = stream;
    public long relativeOffset;
}


public class SourceReadAction(ulong length) : IAction
{
    public async Task Apply(BpsStream src, BpsStream tgt)
    {
        var srcPos = src.stream.Position;
        src.stream.Seek(tgt.stream.Position, SeekOrigin.Begin);

        byte[] data = new byte[length];
        await src.stream.ReadExactlyAsync(data);

        await tgt.stream.WriteAsync(data);

        src.stream.Seek(srcPos, SeekOrigin.Begin);
    }
}

public class TargetReadAction(byte[] data) : IAction
{
    public async Task Apply(BpsStream src, BpsStream tgt)
    {
        await tgt.stream.WriteAsync(data);
    }
}

public class SourceCopyAction(ulong length, long sourceOffset) : IAction
{
    public async Task Apply(BpsStream src, BpsStream tgt)
    {
        src.relativeOffset += sourceOffset;
        src.stream.Seek(src.relativeOffset, SeekOrigin.Begin);

        byte[] data = new byte[length];
        await src.stream.ReadExactlyAsync(data);

        await tgt.stream.WriteAsync(data);

        src.relativeOffset += (long)length;
    }
}
public class TargetCopyAction(ulong length, long targetOffset) : IAction
{
    public async Task Apply(BpsStream src, BpsStream tgt)
    {
        tgt.relativeOffset += targetOffset;

        long remaining = (long)length;
        while (remaining > 0)
        {
            var tgtPosition = tgt.stream.Position;
            tgt.stream.Seek(tgt.relativeOffset, SeekOrigin.Begin);

            var readLength = Math.Min(tgt.stream.Length - tgt.stream.Position, remaining);
            byte[] data = new byte[readLength];
            await tgt.stream.ReadExactlyAsync(data);

            tgt.stream.Seek(tgtPosition, SeekOrigin.Begin);

            // Special handling of case in which patch is reading the last byte of target and writing it
            // to target itself. This means that patch actually wants to append the same byte remaining times
            // to the target. Append repeated bytes all at once to speed up patching.
            if (readLength == 1)
            {
                await AppendLastByte(data[0], (int)remaining, tgt.stream);
                tgt.relativeOffset += remaining;
                break;
            }

            await tgt.stream.WriteAsync(data);
            tgt.relativeOffset += readLength;

            remaining -= readLength;
        }
    }

    private static async Task AppendLastByte(byte data, int count, Stream tgt)
    {
        await tgt.WriteAsync(Enumerable.Repeat(data, count).ToArray());
    } 
}