using CommunityToolkit.Mvvm.ComponentModel;

using Modthara.App.Routing;

namespace Modthara.App.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly Router<ViewModelBase> _router;

    [ObservableProperty] private SidebarItemViewModel? _selectedItem;

    [ObservableProperty]
    private ViewModelBase? _content;

    public static IReadOnlyList<SidebarItemViewModel> SidebarItems => [
        // TODO: implement home view
        // new("Home", "home", "fa-solid fa-home"),
        new("Packages", "packages", "fa-solid fa-box"),
#if DEBUG // TODO: implement the rest of the views
        new("Overrides", "overrides", "fa-solid fa-folder"),
        new("Native Mods", "nativeMods", "fa-solid fa-cubes"),
        new("Settings", "settings", "fa-solid fa-wrench"),
        new("About", "about", "fa-solid fa-circle-info")
#endif
    ];

    public MainViewModel(Router<ViewModelBase> router)
    {
        _router = router;

        router.CurrentViewModelChanged += viewModel => Content = viewModel;
        SelectedItem = SidebarItems[0];
    }

    partial void OnSelectedItemChanged(SidebarItemViewModel? value)
    {
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
        }
    }
}
