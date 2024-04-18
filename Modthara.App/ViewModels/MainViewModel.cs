using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Modthara.App.Models;
using Modthara.App.Routing;

namespace Modthara.App.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly Router<ViewModelBase> _router;

    [ObservableProperty] private SidebarItem? _selectedItem;

    [ObservableProperty]
    private ViewModelBase? _content;

    public static IReadOnlyList<SidebarItem> SidebarItems => [
        new("Home", "home", "fa-solid fa-home"),
        new("Packages", "packages", "fa-solid fa-box"),
        new("Overrides", "overrides", "fa-solid fa-folder"),
        new("Native Mods", "nativeMods", "fa-solid fa-cubes")
    ];

    public MainViewModel(Router<ViewModelBase> router)
    {
        _router = router;

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
}
