using System.IO.Hashing;

namespace RomPatcher;

public class Patcher(string patchFile, Stream source, string targetRom)
{

    // Create patched rom and return its crc32 checksum
    public async Task<uint> ApplyPatch()
    {
        using var bps = await Bps.Create(patchFile);

        if (!bps.IsValid())
            throw new ArgumentException("Invalid BPS file", patchFile);

        // Verify source
        var sourceChecksum = await ComputeChecksum(source);
        if (bps.SourceChecksum != sourceChecksum)
            throw new ArgumentException("Invalid source ROM");

        // Prepare target
        using var target = File.Open(targetRom, FileMode.Create);
        target.Seek(0, SeekOrigin.Begin);

        var sourceStream = new BpsStream(source);
        var targetStream = new BpsStream(target);

        while (bps.HasNext())
        {
            var action = await bps.Next();
            await action.Apply(sourceStream, targetStream);
        }

        uint targetChecksum = await ComputeChecksum(target);

        if (targetChecksum != bps.TargetChecksum)
        {
            File.Delete(targetRom);
            throw new Exception("Generated wrong invalid patched ROM");

        }

        return targetChecksum;
    }

    public static async Task<uint> GetSourceCrc32(string patchFile)
    {
        using var bps = await Bps.Create(patchFile);
        return bps.SourceChecksum;
    }

    public static async Task<uint> ComputeChecksum(Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);

        var buffer = new byte[stream.Length];
        await stream.ReadExactlyAsync(buffer);
        stream.Seek(0, SeekOrigin.Begin);

        return Crc32.HashToUInt32(buffer);
    }
}
