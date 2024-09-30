using System.IO.Abstractions;

using AsyncAwaitBestPractices;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;

using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;

using Microsoft.Extensions.DependencyInjection;

using Modthara.Manager;
using Modthara.UI.Routing;
using Modthara.UI.Services;
using Modthara.UI.ViewModels;
using Modthara.UI.Views;

namespace Modthara.UI;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        BindingPlugins.DataValidators.RemoveAt(0);

        ServiceProvider services = ConfigureServices();
        Ioc.Default.ConfigureServices(services);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow { DataContext = services.GetRequiredService<MainViewModel>() };
        }

        EnsurePaths();

        services.GetRequiredService<ConfigurationService>().LoadConfiguration();

        services
            .GetRequiredService<PackagesViewModel>()
            .InitializeViewModelAsync()
            .SafeFireAndForget();

        base.OnFrameworkInitializationCompleted();
    }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<Router<ViewModelBase>>(s =>
            new Router<ViewModelBase>(v => (ViewModelBase)s.GetRequiredService(v)));

        services.AddSingleton<IFileSystem, FileSystem>();
        services.AddSingleton<IProcessProxy, ProcessProxy>();
        // TODO: each OS target should have its own implementation factory with platform specific paths
        services.AddSingleton<ConfigurationService>(p => new ConfigurationService(p.GetRequiredService<IFileSystem>(),
            ConfigPath, DefaultWindowsConfigurationModel));
        services.AddSingleton<IPathProvider>(p => p.GetRequiredService<ConfigurationService>());
        services.AddSingleton<ModPackageService>();
        services.AddSingleton<ModSettingsService>();
        services.AddSingleton<ModManager>();

        services.AddSingleton<MainViewModel>();
        services.AddSingleton<PackagesViewModel>();

        return services.BuildServiceProvider();
    }

    private static void EnsurePaths()
    {
        if (!Directory.Exists(AppDataPath))
        {
            Directory.CreateDirectory(AppDataPath);
        }

        if (!File.Exists(ConfigPath))
        {
            // create a config with default values
            Ioc.Default.GetRequiredService<ConfigurationService>().SaveConfiguration();
        }
    }

    private static readonly string AppDataPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "modthara");

    private static readonly string ConfigPath = Path.Combine(AppDataPath, "config.json");

    // TODO: windows specific portion to be moved to its own target project
    private static readonly ConfigurationModel DefaultWindowsConfigurationModel = new()
    {
        LocalLarianFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Larian Studios")
    };
}
