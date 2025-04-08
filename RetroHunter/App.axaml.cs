using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Classic.CommonControls.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RetroHunter.Persistence;
using RetroHunter.Services;
using RetroHunter.ViewModels;
using RetroHunter.Views;
using System.Linq;
using System.Threading.Tasks;

namespace RetroHunter;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        // DI container
        var collection = new ServiceCollection();

        collection.AddLogging(builder => builder.AddConsole());

        collection.AddSingleton<Ra2DatContext>();
        collection.AddSingleton<RaClient>();
        collection.AddSingleton<Chdman>();

        collection.AddTransient<MainViewModel>();
        collection.AddTransient<ConfigureDialogViewModel>();

        var services = collection.BuildServiceProvider();
        var vm = services.GetRequiredService<MainViewModel>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = vm
            };

        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = vm
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static Window CurrentWindow()
    {
        var desktop = Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

        return desktop!.Windows.First(w => w.IsActive);

    }

    public static Window MainWindow()
    {
        var desktop = Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        return desktop!.MainWindow!;
    }

    public static async Task ShowError(string title, string description)
    {
        await MessageBox.ShowDialog(MainWindow(), description, title, MessageBoxButtons.Ok, MessageBoxIcon.Error);
    }
    public static async Task ShowInfo(string title, string description)
    {
        await MessageBox.ShowDialog(MainWindow(), description, title, MessageBoxButtons.Ok, MessageBoxIcon.Information);
    }
}
