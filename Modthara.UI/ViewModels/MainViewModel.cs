using System.Diagnostics.CodeAnalysis;

using CommunityToolkit.Mvvm.ComponentModel;

using Modthara.UI.Routing;

namespace Modthara.UI.ViewModels;

public record SidebarItem(string Header, string IconValue, Action<Router<ViewModelBase>> OnSelected);

public partial class MainViewModel : ViewModelBase
{
    private readonly Router<ViewModelBase> _router;

    [ObservableProperty]
    private SidebarItem _selectedItem;

    [ObservableProperty]
    private ViewModelBase? _content;

    public MainViewModel(Router<ViewModelBase> router)
    {
        _router = router;

        router.CurrentViewModelChanged += viewModel => Content = viewModel;

        SelectedItem = SidebarItems[0];
    }

    public static List<SidebarItem> SidebarItems { get; } =
    [
        new("Packages", "fa-solid fa-box", r => r.GoTo<PackagesViewModel>())
    ];

    partial void OnSelectedItemChanged(SidebarItem? value) => value?.OnSelected(_router);
}
