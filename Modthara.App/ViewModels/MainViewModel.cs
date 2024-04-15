using CommunityToolkit.Mvvm.ComponentModel;

using Modthara.App.Routing;

namespace Modthara.App.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly Router<ViewModelBase> _router;

    [ObservableProperty]
    private IReadOnlyList<SidebarPaneItemViewModel> _sidebarPaneItems = [
        new("Home", "home", "fa-solid fa-home"),
        new("Packages", "packages", "fa-solid fa-box"),
        new("Overrides", "overrides", "fa-solid fa-folder"),
        new("Native Mods", "nativeMods", "fa-solid fa-cubes"),
        new("Settings", "settings", "fa-solid fa-wrench"),
        new("About", "about", "fa-solid fa-circle-info")
    ];

    [ObservableProperty] private SidebarPaneItemViewModel? _selectedItem;

    partial void OnSelectedItemChanged(SidebarPaneItemViewModel? value)
    {
        // TODO: Use keyed services or recursion for navigation
        switch (value?.Route)
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
                break;
            case "about":
                _router.GoTo<AboutViewModel>();
                break;
            default:
                _router.GoTo<BlankViewModel>();
                break;
        }
    }

    [ObservableProperty]
    private ViewModelBase? _content;

    public MainViewModel(Router<ViewModelBase> router)
    {
        _router = router;

        router.CurrentViewModelChanged += viewModel => Content = viewModel;
    }
}
