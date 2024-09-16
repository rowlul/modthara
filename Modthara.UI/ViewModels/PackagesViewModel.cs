using System.Collections;
using System.Collections.ObjectModel;

using Avalonia.Collections;
using Avalonia.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;

using Modthara.Lari;
using Modthara.Manager;
using Modthara.UI.Extensions;
using Modthara.UI.Services;

namespace Modthara.UI.ViewModels;

public partial class PackagesViewModel : ViewModelBase
{
    private ModManager ModManager { get; } = Ioc.Default.GetRequiredService<ModManager>();
    private IPathProvider PathProvider { get; } = Ioc.Default.GetRequiredService<IPathProvider>();
    private IProcessProxy Process { get; } = Ioc.Default.GetRequiredService<IProcessProxy>();

    [ObservableProperty]
    private bool _isViewReady;

    [ObservableProperty]
    private ObservableCollection<ModPackageViewModel> _mods = null!;

    [ObservableProperty]
    private DataGridCollectionView _modsView = null!;

    [ObservableProperty]
    private bool _modSearchVisibility;

    [ObservableProperty]
    private string? _modSearchText;

    partial void OnModSearchVisibilityChanged(bool value)
    {
        if (value == false && !string.IsNullOrWhiteSpace(ModSearchText))
        {
            ModSearchText = string.Empty;
        }
    }

    partial void OnModSearchTextChanged(string? oldValue, string? newValue)
    {
        // TODO: introduce
        newValue = newValue?.Trim();

        if (newValue == string.Empty)
        {
            ModsView.Filter = null;
        }
        else if (oldValue == newValue || newValue == null || newValue.IsWhiteSpace())
        {
        }
        else
        {
            ModsView.Filter = x =>
                ((ModPackageViewModel)x).Name.Contains(newValue, StringComparison.OrdinalIgnoreCase);
        }
    }

    public async Task InitializeViewModelAsync()
    {
        await ModManager.LoadModsAsync();

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var (found, missing) = ModManager.MatchMods();
            ;

            Mods = new ObservableCollection<ModPackageViewModel>(found
                .Union(ModManager.ModPackages.DistinctBy(m => m.Metadata?.Uuid ?? LariUuid.NewGuid()))
                .Select(m => new ModPackageViewModel(m)));
            ModsView = new DataGridCollectionView(Mods);
        }, DispatcherPriority.Background);

        IsViewReady = true;
    }

    public void MoveAfterEnabledMods(ModPackageViewModel mod, int shift = 0)
    {
        var index = Mods.IndexOf(mod);
        var last = Mods.LastOrDefault(m => m.IsEnabled && m != mod);
        var end = Mods.IndexOf(last!);

        if (index > end)
        {
            Mods.Move(index, end + 1 + shift);
        }
        else
        {
            Mods.Move(index, end + shift);
        }

        ModsView.Refresh();
    }

    [RelayCommand]
    private void OpenModsFolder()
    {
        Process.OpenFolder(PathProvider.ModsFolder);
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsViewReady = false;
        ModSearchVisibility = false;
        await InitializeViewModelAsync();
    }

    [RelayCommand]
    private void DeleteSelection(IList selectedItems)
    {
        // prevent iterating over the same collection
        foreach (var mod in selectedItems.Cast<ModPackageViewModel>().ToArray())
        {
            mod.DisableModule();
            mod.DeleteModPackage();
            Mods.Remove(mod);
        }
    }

    [RelayCommand]
    private async Task MoveSelectionTopAsync(IList selectedItems)
    {
        for (int i = 0; i < selectedItems.Count; i++)
        {
            var mod = (ModPackageViewModel)selectedItems[i]!;
            if (!mod.IsEnabled || !mod.HasMetadata)
            {
                continue;
            }

            var index = Mods.IndexOf(mod);
            if (index > 0)
            {
                int newIndex = 0;
                if (Mods[newIndex].IsEnabled)
                {
                    ModManager.MoveModule(index, newIndex);
                    Mods.Move(index, newIndex);
                }
            }
        }

        await ModManager.SaveModSettingsAsync();
        ModsView.Refresh();
    }

    [RelayCommand]
    private async Task MoveSelectionUpAsync(IList selectedItems)
    {
        for (int i = 0; i < selectedItems.Count; i++)
        {
            var mod = (ModPackageViewModel)selectedItems[i]!;
            if (!mod.IsEnabled || !mod.HasMetadata)
            {
                continue;
            }

            var index = Mods.IndexOf(mod);
            if (index > 0)
            {
                int newIndex = index - 1;
                if (Mods[newIndex].IsEnabled)
                {
                    ModManager.MoveModule(index, newIndex);
                    Mods.Move(index, newIndex);
                }
            }
        }

        await ModManager.SaveModSettingsAsync();
        ModsView.Refresh();
    }

    [RelayCommand]
    private async Task MoveSelectionDownAsync(IList selectedItems)
    {
        for (int i = selectedItems.Count - 1; i >= 0; i--)
        {
            var mod = (ModPackageViewModel)selectedItems[i]!;
            if (!mod.IsEnabled || !mod.HasMetadata)
            {
                continue;
            }

            var index = Mods.IndexOf(mod);
            if (index < Mods.Count - 1)
            {
                int newIndex = index + 1;
                if (Mods[newIndex].IsEnabled)
                {
                    ModManager.MoveModule(index, newIndex);
                    Mods.Move(index, newIndex);
                }
            }
        }

        await ModManager.SaveModSettingsAsync();
        ModsView.Refresh();
    }

    [RelayCommand]
    private async Task MoveSelectionBottomAsync(IList selectedItems)
    {
        var last = Mods.LastOrDefault(m => m.IsEnabled);
        if (last == null)
        {
            return;
        }

        var end = Mods.IndexOf(last);
        for (int i = selectedItems.Count - 1; i >= 0; i--)
        {
            var mod = (ModPackageViewModel)selectedItems[i]!;
            if (!mod.IsEnabled || !mod.HasMetadata)
            {
                continue;
            }

            var index = Mods.IndexOf(mod);
            if (index < end)
            {
                ModManager.MoveModule(index, end);
                Mods.Move(index, end);
                end--;
            }
        }

        await ModManager.SaveModSettingsAsync();
        ModsView.Refresh();
    }
}
