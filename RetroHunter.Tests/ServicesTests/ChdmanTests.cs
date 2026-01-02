

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RetroHunter.Services;
using RetroHunter.Utils;

namespace RetroHunter.Tests.ChdmanTests;

public class ChdmanTests(ITestOutputHelper outputHelper)
{
    private readonly ServiceProvider _serviceProvider = new ServiceCollection()
           .AddLogging((builder) => builder.AddXUnit(outputHelper))
           .BuildServiceProvider();

    [Theory]
    [InlineData("../../../TestRoms/ps1/007 Racing (USA).cue")]
    public async Task CompressCD(string inputPath)
    {
        var inInfo = new FileInfo(inputPath);

        var outPath = inInfo.FullName.Replace(inInfo.Extension, "_out.chd");

        var chdmanPath = DirUtils.FindTool("chdman");
        Assert.NotEqual("", chdmanPath);

        var chdman = new Chdman(null, chdmanPath);

        await chdman.Compress(Chdman.CompressType.CD, inputPath, outPath);

        Assert.True(File.Exists(outPath));

        File.Delete(outPath);
    }
}