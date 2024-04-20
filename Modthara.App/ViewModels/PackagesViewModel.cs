using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using Modthara.Essentials.Packaging;

namespace Modthara.App.ViewModels;

public partial class PackagesViewModel : ViewModelBase
{
    [ObservableProperty] private bool _isFlagsColumnVisible;
    [ObservableProperty] private bool _isVersionColumnVisible;
    [ObservableProperty] private bool _isAuthorColumnVisible;
    [ObservableProperty] private bool _isLastModifiedColumnVisible;

    [ObservableProperty] private ObservableCollection<ModPackage>? _mods;

    public PackagesViewModel()
    {
        IsFlagsColumnVisible = true;
        IsVersionColumnVisible = true;
        IsAuthorColumnVisible = true;
        IsLastModifiedColumnVisible = true;
    }
}
