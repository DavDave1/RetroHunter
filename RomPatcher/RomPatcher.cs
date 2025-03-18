using System.IO.Hashing;

namespace RomPatcher;

public class Patcher(string patchFile, string sourceRom, string targetRom)
{

    public bool ApplyPatch()
    {

        var patch = new Bps(patchFile);

        if (!patch.IsValid())
        {
            return false;
        }

        // Verify source
        using var source = File.OpenRead(sourceRom);

        if (patch.SourceChecksum != ComputeChecksum(source))
        {
            return false;
        }


        // Prepare target
        using var target = File.Open(targetRom, FileMode.Create);
        target.Seek(0, SeekOrigin.Begin);


        BpsStream sourceStream = new BpsStream(source);
        BpsStream targetStream = new BpsStream(target);

        while (patch.HasNext())
        {
            var action = patch.Next();
            action.Apply(sourceStream, targetStream);
        }

        return ComputeChecksum(target) == patch.TargetChecksum;
    }

    private static uint ComputeChecksum(Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);

        var buffer = new byte[stream.Length];
        stream.ReadExactly(buffer);
        stream.Seek(0, SeekOrigin.Begin);

        return Crc32.HashToUInt32(buffer);
    }

}
