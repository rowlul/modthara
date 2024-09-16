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
        services.AddSingleton<IPathProvider, WindowsDefaultPaths>();
        services.AddSingleton<ModPackageService>();
        services.AddSingleton<ModSettingsService>();
        services.AddSingleton<ModManager>();

        services.AddSingleton<MainViewModel>();
        services.AddSingleton<PackagesViewModel>();

        return services.BuildServiceProvider();
    }
}
