using System.Collections.ObjectModel;
using System.IO.Abstractions;

using AsyncAwaitBestPractices;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

using CommunityToolkit.Mvvm.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

using Modthara.App.Routing;
using Modthara.App.ViewModels;
using Modthara.App.Views;
using Modthara.Essentials.Packaging;

namespace Modthara.App;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // https://github.com/AvaloniaUI/Avalonia/discussions/10239
        BindingPlugins.DataValidators.RemoveAt(0);

        ServiceProvider services = ConfigureServices();
        Ioc.Default.ConfigureServices(services);

        var mainVm = services.GetRequiredService<MainViewModel>();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow { DataContext = mainVm };
        }

        mainVm.LoadPackages().ContinueWith(_ =>
        {
            var modsDirectory = services.GetRequiredService<IModsService>();
            var packagesVm = services.GetRequiredService<PackagesViewModel>();
            packagesVm.Mods = new ObservableCollection<ModPackage>(modsDirectory.ModPackages);
        }).SafeFireAndForget();

        base.OnFrameworkInitializationCompleted();
    }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<Router<ViewModelBase>>(s =>
            new Router<ViewModelBase>(v => (ViewModelBase)s.GetRequiredService(v)));

        services.AddScoped<IFileSystem, FileSystem>();
        services.AddTransient<IModPackageManager, ModPackageManager>();
        services.AddSingleton<IModsService>(s =>
            new ModsService(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                @"\Larian Studios\Baldur's Gate 3\Mods",
                s.GetRequiredService<IModPackageManager>(),
                s.GetRequiredService<IFileSystem>()));

        services.AddSingleton<MainViewModel>();
        services.AddSingleton<PackagesViewModel>();

        services.AddTransient<BlankViewModel>();
        services.AddTransient<HomeViewModel>();
        services.AddTransient<OverridesViewModel>();
        services.AddTransient<NativeModsViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<AboutViewModel>();

        return services.BuildServiceProvider();
    }
}
