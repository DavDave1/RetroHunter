
using System.Reflection.PortableExecutable;
using Avalonia.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RetroHunter.Services;

namespace RetroHunter.Tests.ChdmanTests;

public class ChdmanTests
{
    private ServiceProvider _serviceProvider;

    public ChdmanTests(ITestOutputHelper outputHelper)
    {
        _serviceProvider = new ServiceCollection()
           .AddLogging((builder) => builder.AddXUnit(outputHelper))
           .AddTransient<Chdman>()
           .BuildServiceProvider();
    }
    [Fact]
    public void DetectChdmanInPath()
    {
        var chdman = _serviceProvider.GetService<Chdman>()!;
        Assert.True(chdman.Detect());
    }

    [Theory]
    [InlineData("../../../TestRoms/ps1/007 Racing (USA).cue")]
    public async Task CompressCD(string inputPath)
    {
        var inInfo = new FileInfo(inputPath);

        var outPath = inInfo.FullName.Replace(inInfo.Extension, "_out.chd");

        var chdman = _serviceProvider.GetService<Chdman>()!;
        chdman.Detect();

        bool ok = await chdman.Compress(Chdman.CompressType.CD, inputPath, outPath);
        Assert.True(ok);


        Assert.True(File.Exists(outPath));

        File.Delete(outPath);
    }
}