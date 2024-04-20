using System.Collections.ObjectModel;
using System.IO.Abstractions;

using AsyncAwaitBestPractices;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;

using CommunityToolkit.Mvvm.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

using Modthara.App.Routing;
using Modthara.App.ViewModels;
using Modthara.App.Views;
using Modthara.Essentials.Abstractions;
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

        var viewModel = services.GetRequiredService<MainViewModel>();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow { DataContext = viewModel };
        }

        viewModel.LoadPackages().ContinueWith(_ =>
        {
            var packager = services.GetRequiredService<IPackager>();
            if (packager.CachedModCount > 0)
            {
                var packagesVm = services.GetRequiredService<PackagesViewModel>();
                packagesVm.Mods = new ObservableCollection<ModPackage>(packager.Cache);
            }
        }).SafeFireAndForget();

        base.OnFrameworkInitializationCompleted();
    }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<Router<ViewModelBase>>(s =>
            new Router<ViewModelBase>(v => (ViewModelBase)s.GetRequiredService(v)));
        services.AddSingleton<PaginatedRouter<ViewModelBase>>(s =>
            new PaginatedRouter<ViewModelBase>(v => (ViewModelBase)s.GetRequiredService(v)));

        services.AddSingleton<IFileSystem, FileSystem>();
        services.AddSingleton<IPackager>(s =>
            new Packager(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                @"\Larian Studios\Baldur's Gate 3\Mods",
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
