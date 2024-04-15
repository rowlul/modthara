using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;

using CommunityToolkit.Mvvm.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

using Modthara.App.Routing;
using Modthara.App.ViewModels;
using Modthara.App.Views;

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

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var viewModel = services.GetRequiredService<MainViewModel>();
            desktop.MainWindow = new MainWindow { DataContext = viewModel };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<Router>(s => new Router(v => (ViewModelBase)s.GetRequiredService(v)));
        services.AddSingleton<PaginatedRouter>(s => new PaginatedRouter(v => (ViewModelBase)s.GetRequiredService(v)));
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<BlankViewModel>();

        return services.BuildServiceProvider();
    }
}
