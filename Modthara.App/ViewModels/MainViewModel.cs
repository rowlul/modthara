using System.Collections.ObjectModel;

using AsyncAwaitBestPractices;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Modthara.App.Models;
using Modthara.App.Routing;
using Modthara.Essentials.Abstractions;
using Modthara.Essentials.Packaging;

namespace Modthara.App.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly Router<ViewModelBase> _router;
    private readonly IPackager _packager;

    [ObservableProperty] private string _progressStatus;
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
                HandlePackages().SafeFireAndForget();
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

    private async Task HandlePackages()
    {
        var vm = _router.GoTo<PackagesViewModel>();

        if (vm.Mods != null || IsBusy)
        {
            return;
        }

        var mods = new ObservableCollection<ModPackage>();
        var modCount = await _packager.CountAsync();

        int i = 0;
        await using var e = _packager.ReadPackagesAsync().GetAsyncEnumerator();
        while (await e.MoveNextAsync())
        {
            i++;

            IsBusy = true;
            IsProgressIndeterminate = true;
            ProgressStatus = $"({i}/{modCount}) Processing package: {e.Current.Name}";

            mods.Add(e.Current);
        }

        vm.Mods = mods;

        IsBusy = false;
        IsProgressIndeterminate = false;
    }
}
