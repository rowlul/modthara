using System.Collections.ObjectModel;
using System.Diagnostics;

using Avalonia.Collections;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Modthara.Lari;
using Modthara.Manager;
using Modthara.UI.Extensions;

namespace Modthara.UI.ViewModels;

public partial class PackagesViewModel : ViewModelBase
{
    private readonly IModsService _modsService;
    private readonly IModSettingsService _modSettingsService;

    private readonly IModManagerSettingsService _modManagerSettingsService;

    [ObservableProperty]
    private bool _isViewReady;

    #region Library Mods

    [ObservableProperty]
    private ObservableCollection<ModPackageViewModel> _libraryMods;

    [ObservableProperty]
    private DataGridCollectionView _libraryModsView;

    [ObservableProperty]
    private bool _libraryModsSearchVisibility;

    [ObservableProperty]
    private string? _libraryModsSearchText;

    partial void OnLibraryModsSearchVisibilityChanged(bool value)
    {
        if (value == false && !string.IsNullOrWhiteSpace(LibraryModsSearchText))
        {
            LibraryModsSearchText = string.Empty;
        }
    }

    partial void OnLibraryModsSearchTextChanged(string? oldValue, string? newValue)
    {
        newValue = newValue?.Trim();

        if (newValue == string.Empty)
        {
            LibraryModsView.Filter = SelectedPackageCategory.ToFilter();
        }
        else if (oldValue == newValue || newValue == null || newValue.IsWhiteSpace())
        {
        }
        else
        {
            LibraryModsView.Filter = SelectedPackageCategory.ToFilter(newValue);
        }
    }

    [RelayCommand]
    private void OpenModsFolder()
    {
        try {
            var path = _modManagerSettingsService.GetModsDirectoryPath();
            var attributes = File.GetAttributes(path);
            // This check is important, otherwise, a maliciously crafted settings file could
            // make the following code execute an arbitrary executable.
            if ((attributes & FileAttributes.Directory) == FileAttributes.Directory) {
                Process p = new Process();
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.FileName = path;
                p.Start();
            } else {
                throw new Exception("ModsPath not a directory"); 
            }
            
        } catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    [RelayCommand]
    private void EnableAllMods()
    {
        foreach (var mod in LibraryMods)
        {
            if (!mod.IsEnabled)
            {
                mod.Enable();
            }
        }
    }

    [RelayCommand]
    private void EnableAllOverrides()
    {
        foreach (var mod in LibraryMods.Where(x => x.IsPureOverride))
        {
            if (!mod.IsEnabled)
            {
                mod.Enable();
            }
        }
    }

    [RelayCommand]
    private void DisableAllMods()
    {
        foreach (var mod in LibraryMods)
        {
            if (mod.IsEnabled)
            {
                mod.Disable();
            }
        }
    }

    [RelayCommand]
    private void DisableAllOverrides()
    {
        foreach (var mod in LibraryMods.Where(x => x.IsPureOverride))
        {
            if (mod.IsEnabled)
            {
                mod.Disable();
            }
        }
    }

    #endregion

    #region Order Mods

    [ObservableProperty]
    private ObservableCollection<ModPackageViewModel> _orderMods;

    [ObservableProperty]
    private DataGridCollectionView _orderModsView;

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

    partial void OnOrderSearchTextChanged(string? oldValue, string? newValue)
    {
        newValue = newValue?.Trim();

        if (newValue == string.Empty)
        {
            OrderModsView.Filter = null;
        }
        else if (oldValue == newValue || newValue == null || newValue.IsWhiteSpace())
        {
        }
        else
        {
            OrderModsView.Filter = x =>
                ((ModPackageViewModel)x).Name.Contains(newValue, StringComparison.OrdinalIgnoreCase);
        }
    }

    [RelayCommand]
    public void OnUpdateClick()
    {

        // Remove all mods in mod order to make their order irrelevant
        foreach (var mod in OrderMods)
        {
            if(mod._modPackage.Metadata is not null)
            {
                _modSettingsService.ModSettings.Remove(mod._modPackage.Metadata);
            }
            
        }

        // Remove all mods in library to disable mods removed from mod order by the user
        foreach (var mod in LibraryMods)
        {
            if(mod._modPackage.Metadata is not null)
            {
                _modSettingsService.ModSettings.Remove(mod._modPackage.Metadata);
            }
            
        }

        // Add back, in sequential order, mods in the mod order
        foreach (var mod in OrderMods)
        {
            if(mod._modPackage.Metadata is not null)
            {
                _modSettingsService.ModSettings.Append(mod._modPackage.Metadata);
            }
            
        }

        _modSettingsService.SaveModSettingsAsync().Wait();
    }

    #endregion

    #region Package category

    [ObservableProperty]
    private PackageCategory _selectedPackageCategory;

    [RelayCommand]
    private void SetPackageCategory(PackageCategory category)
    {
        SelectedPackageCategory = category;

        if (!string.IsNullOrWhiteSpace(LibraryModsSearchText))
        {
            LibraryModsSearchText = string.Empty;
        }
    }

    partial void OnSelectedPackageCategoryChanged(PackageCategory value) => LibraryModsView.Filter = value.ToFilter();

    #endregion

    public PackagesViewModel(
        IModsService modsService,
        IModSettingsService modSettingsService,
        IModManagerSettingsService modManagerSettingsService)
    {
        _modsService = modsService;
        _modSettingsService = modSettingsService;
        _modManagerSettingsService = modManagerSettingsService;
    }

    public async Task InitializeViewModel()
    {
        await _modManagerSettingsService.LoadSettingsAsync();
        _modsService.Path = _modManagerSettingsService.GetModsDirectoryPath();
        _modSettingsService.Path = _modManagerSettingsService.GetModSettingsPath();

        await Task.WhenAll(_modsService.LoadModPackagesAsync(), _modSettingsService.LoadModSettingsAsync());

        var (foundOrderMods, missingOrderMods) =
            await _modsService.GetModsFromModSettingsAsync(_modSettingsService.ModSettings);

        await Task.WhenAll(
            Task.Run(() =>
            {
                var mods = _modsService.ModPackages
                    .Except(foundOrderMods)
                    .Select(x => new ModPackageViewModel(x));
                LibraryMods = new ObservableCollection<ModPackageViewModel>(mods);
            }),
            Task.Run(() =>
            {
                var mods = foundOrderMods.Select(x => new ModPackageViewModel(x));
                OrderMods = new ObservableCollection<ModPackageViewModel>(mods);
            }));

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            OrderModsView = new DataGridCollectionView(OrderMods);
            LibraryModsView = new DataGridCollectionView(LibraryMods);
        }, DispatcherPriority.Background);

        SelectedPackageCategory = PackageCategory.Standalone;
        IsViewReady = true;
    }
}
