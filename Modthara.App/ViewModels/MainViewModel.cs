using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Modthara.App.Models;
using Modthara.App.Routing;
using Modthara.Essentials.Abstractions;

namespace Modthara.App.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly Router<ViewModelBase> _router;
    private readonly IPackager _packager;

    [ObservableProperty] private string _progressStatus = string.Empty;
    [ObservableProperty] private double _progressMax = 100;
    [ObservableProperty] private double _progressValue;
    [ObservableProperty] private bool _isProgressIndeterminate;

    [ObservableProperty] private bool _isBusy;

    [ObservableProperty] private SidebarItem? _selectedItem;
    [ObservableProperty] private ViewModelBase? _content;

    public static IReadOnlyList<SidebarItem> SidebarItems =>
    [
        new("Home", "home", "fa-solid fa-home"),
        new("Packages", "packages", "fa-solid fa-box"),
        new("Overrides", "overrides", "fa-solid fa-folder"),
        new("Native Mods", "nativeMods", "fa-solid fa-cubes")
    ];

    public MainViewModel(Router<ViewModelBase> router, IPackager packager)
    {
        _router = router;
        _packager = packager;

        router.CurrentViewModelChanged += viewModel => Content = viewModel;
        SelectedItem = SidebarItems[0];
    }

    partial void OnSelectedItemChanged(SidebarItem? value) => Navigate(value?.Route);

    [RelayCommand]
    private void Navigate(string? route)
    {
        switch (route)
        {
            case "home":
                _router.GoTo<HomeViewModel>();
                break;
            case "packages":
                _router.GoTo<PackagesViewModel>();
                break;
            case "overrides":
                _router.GoTo<OverridesViewModel>();
                break;
            case "nativeMods":
                _router.GoTo<NativeModsViewModel>();
                break;
            case "settings":
                _router.GoTo<SettingsViewModel>();
                SelectedItem = null;
                break;
            case "about":
                _router.GoTo<AboutViewModel>();
                SelectedItem = null;
                break;
        }
    }

    public async Task LoadPackages()
    {
        if (IsBusy)
        {
            return;
        }

        var modCount = await _packager.CountAsync();
        if (modCount == 0)
        {
            return;
        }

        IsBusy = true;
        IsProgressIndeterminate = true;

        await _packager.LoadPackagesToCacheAsync((idx, pak) =>
        {
            ProgressStatus = $"({idx}/{modCount}) Processing package: {pak.Name}";
            return Task.CompletedTask;
        }, e =>
        {
            // TODO: use serilog and user-friendly notifications
            Console.WriteLine(e);
            return Task.CompletedTask;
        });

        IsBusy = false;
        IsProgressIndeterminate = false;
    }
}
