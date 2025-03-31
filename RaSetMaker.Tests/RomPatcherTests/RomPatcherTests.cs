
using System.Threading.Tasks;
using RomPatcher;

namespace RaSetMaker.Tests.RomPatcherTests;

public class RomPatcherTest
{

    [Theory]
    [InlineData("../../../TestRoms/patch/test_patch.bps")]
    public async Task ReadBpsPatch(string patch)
    {
        var bps = await Bps.Create(patch);

        Assert.True(bps.IsValid());
    }

    [Theory]
    [InlineData("../../../TestRoms/patch/test_patch.bps", "../../../TestRoms/patch/source.gba")]
    public async Task ApplyPatch(string patch, string source)
    {
        var target = "../../../TestRoms/patch/target.gba";
        var patcher = new Patcher(patch, source, target);
        await patcher.ApplyPatch();

        Assert.True(File.Exists(target));
    }
}
