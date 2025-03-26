using System.IO.Hashing;

namespace RomPatcher;

public class Patcher(string patchFile, string sourceRom, string targetRom)
{

    public async Task<bool> ApplyPatch()
    {
        var bps = await Bps.Create(patchFile);

        if (!bps.IsValid())
        {
            return false;
        }

        // Verify source
        using var source = File.OpenRead(sourceRom);

        var sourceChecksum = await ComputeChecksum(source);
        if (bps.SourceChecksum != sourceChecksum)
        {
            return false;
        }


        // Prepare target
        using var target = File.Open(targetRom, FileMode.Create);
        target.Seek(0, SeekOrigin.Begin);


        BpsStream sourceStream = new BpsStream(source);
        BpsStream targetStream = new BpsStream(target);

        while (bps.HasNext())
        {
            var action = await bps.Next();
            await action.Apply(sourceStream, targetStream);
        }

        return (await ComputeChecksum(target)) == bps.TargetChecksum;
    }

    public static async Task<uint> GetSourceCrc32(string patchFile)
    {
        return (await Bps.Create(patchFile)).SourceChecksum;
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
