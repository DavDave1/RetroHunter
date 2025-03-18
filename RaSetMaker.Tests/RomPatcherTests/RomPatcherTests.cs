
using RomPatcher;

namespace RaSetMaker.Tests.RomPatcherTests;

public class RomPatcherTest
{

    [Theory]
    [InlineData("../../../TestRoms/patch/test_patch.bps")]
    public void ReadBpsPatch(string patch)
    {
        var bps = new Bps(patch);

        Assert.True(bps.IsValid());
    }

    [Theory]
    [InlineData("../../../TestRoms/patch/test_patch.bps", "../../../TestRoms/patch/source.gba")]
    public void ApplyPatch(string patch, string source)
    {
        var target = "../../../TestRoms/patch/target.gba";
        var patcher = new Patcher(patch, source, target);

        Assert.True(patcher.ApplyPatch());
    }
}
