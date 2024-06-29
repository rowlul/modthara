using Avalonia.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Modthara.App.Services;
using Modthara.Essentials.Packaging;
using Modthara.Lari;

namespace Modthara.App.ViewModels;

using Source = FlatTreeDataGridSource<ModPackageViewModel>;

public partial class PackagesViewModel : ViewModelBase
{
    private readonly IModsService _modsService;
    private readonly IModSettingsService _modSettingsService;
    private readonly IModGridService _modGridService;
    private readonly IOrderGridService _orderGridService;

    [ObservableProperty]
    private bool _isViewReady;

    #region Mods

    [ObservableProperty]
    private Source? _modsSource;

    [ObservableProperty]
    private bool _modSearchVisibility;

    [ObservableProperty]
    private string? _modsSearchText;

    partial void OnModSearchVisibilityChanged(bool value)
    {
        if (value == false && !string.IsNullOrWhiteSpace(ModsSearchText))
        {
            ModsSearchText = string.Empty;
        }
    }

    partial void OnModsSearchTextChanged(string? value)
    {
        var filtered = _modGridService.FilterMods(value, SelectedPackageCategory.ToPredicate());
        if (filtered != null)
        {
            ModsSource.Items = filtered;
        }
    }

    [RelayCommand]
    private void OpenModsFolder()
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private void EnableAllMods()
    {
        _modGridService.EnableMods();
    }

    [RelayCommand]
    private void EnableAllOverrides()
    {
        _modGridService.EnableMods(x => x.HasOverrides && !x.HasModFiles);
    }

    [RelayCommand]
    private void DisableAllMods()
    {
        _modGridService.DisableMods();
    }

    [RelayCommand]
    private void DisableAllOverrides()
    {
        _modGridService.DisableMods(x => x.HasOverrides && !x.HasModFiles);
    }

    #endregion

    #region Order

    [ObservableProperty]
    private Source? _orderSource;

    [ObservableProperty]
    private bool _orderSearchVisibility;

    [ObservableProperty]
    private string? _orderSearchText;

    partial void OnOrderSearchVisibilityChanged(bool value)
    {
        if (value == false && !string.IsNullOrWhiteSpace(OrderSearchText))
        {
            OrderSearchText = string.Empty;
        }
    }

    partial void OnOrderSearchTextChanged(string? value)
    {
        var filtered = _orderGridService.FilterMods(value);
        if (filtered != null)
        {
            OrderSource.Items = filtered;
        }
    }

    #endregion

    #region Package category

    [ObservableProperty]
    private PackageCategory _selectedPackageCategory;

    [RelayCommand]
    private void SetPackageCategory(PackageCategory category)
    {
        SelectedPackageCategory = category;

        if (!string.IsNullOrWhiteSpace(ModsSearchText))
        {
            ModsSearchText = string.Empty;
        }
    }

    partial void OnSelectedPackageCategoryChanged(PackageCategory value)
    {
        var predicate = value.ToPredicate();
        ModsSource.Items = predicate != null ? _modGridService.ViewModels.Where(predicate) : _modGridService.ViewModels;
    }

    #endregion

    public PackagesViewModel(
        IModsService modsService,
        IModSettingsService modSettingsService,
        IModGridService modGridService,
        IOrderGridService orderGridService)
    {
        _modsService = modsService;
        _modSettingsService = modSettingsService;
        _modGridService = modGridService;
        _orderGridService = orderGridService;
    }

    public async Task InitializeViewModel()
    {
        await Task.WhenAll(_modsService.LoadModPackagesAsync(), _modSettingsService.LoadModSettingsAsync());

        await Task.Run(() => _modGridService.CreateViewModels());
        await Task.Run(() =>
        {
            IEnumerable<ModMetadata> missingMods;
            _orderGridService.CreateViewModels(out missingMods);
        });

        await Task.WhenAll(
                Task.Run(() => ModsSource = _modGridService.CreateSource(x => x.HasModFiles)),
                Task.Run(() => OrderSource = _orderGridService.CreateSource()))
            .ContinueWith(_ =>
            {
                IsViewReady = true;
            });
    }
}
