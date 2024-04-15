using CommunityToolkit.Mvvm.ComponentModel;

using Modthara.App.Routing;

namespace Modthara.App.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase? _content;

    public MainViewModel(Router<ViewModelBase> router)
    {
        router.CurrentViewModelChanged += viewModel => Content = viewModel;
    }
}
