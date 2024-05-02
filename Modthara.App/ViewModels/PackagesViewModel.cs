using System.Collections.ObjectModel;
using System.Diagnostics;

using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Humanizer;

using Modthara.Essentials.Packaging;
using Modthara.Lari;

namespace Modthara.App.ViewModels;

public partial class PackagesViewModel : ViewModelBase
{
    private readonly IModsService _modsService;

    [ObservableProperty] private ObservableCollection<ModPackage>? _mods;

    [ObservableProperty] private FlatTreeDataGridSource<ModPackage>? _alteredGameFileModsSource;

    [ObservableProperty] private bool _isToggleOverridesChecked;

    public PackagesViewModel(IModsService modsService)
    {
        _modsService = modsService;
    }

    public ObservableCollection<ModPackage> GetMods() => new(_modsService.ModPackages);

    public FlatTreeDataGridSource<ModPackage>? CreateAlteredGameFileModsSource()
    {
        if (Mods == null)
        {
            return null;
        }

        var source =
            new FlatTreeDataGridSource<ModPackage>(Mods.Where(x =>
                (x.Flags & ModFlags.AltersGameFiles) != ModFlags.None &&
                (x.Flags & ModFlags.HasModFiles) == ModFlags.None))
            {
                Columns =
                {
                    new CheckBoxColumn<ModPackage>(string.Empty,
                        getter: x => x.Flags.HasFlag(ModFlags.Enabled),
                        setter: (x, newValue) =>
                        {
                            if (newValue)
                            {
                                _modsService.EnableModPackage(x);
                                Debug.Assert((x.Flags & ModFlags.Enabled) == ModFlags.Enabled);
                            }
                            else
                            {
                                _modsService.DisableModPackage(x);
                                Debug.Assert((x.Flags & ModFlags.Enabled) == ModFlags.None);
                            }
                        },
                        options: new CheckBoxColumnOptions<ModPackage> { CanUserResizeColumn = false }),
                    new TextColumn<ModPackage, string>("Name",
                        getter: x => x.Name,
                        options: new TextColumnOptions<ModPackage> { IsTextSearchEnabled = true }),
                    new TextColumn<ModPackage, LariVersion>("Version", getter: x => x.Version),
                    new TextColumn<ModPackage, string>("Author",
                        getter: x => x.Author),
                    new TextColumn<ModPackage, string>("Last Modified",
                        getter: x => x.LastModified.Humanize(null, null, null))
                }
            };

        source.RowSelection!.SingleSelect = false;

        return source;
    }

    [RelayCommand(CanExecute = nameof(CanToggleOverridesExecute))]
    private void ToggleOverrides()
    {
        if (IsToggleOverridesChecked)
        {
            for (int i = 0; i < AlteredGameFileModsSource.Rows.Count; i++)
            {
                ((CheckBoxCell)AlteredGameFileModsSource.Rows.RealizeCell(AlteredGameFileModsSource.Columns[0], 0, i))
                    .Value = true;
            }
        }
        else
        {
            for (int i = 0; i < AlteredGameFileModsSource.Rows.Count; i++)
            {
                ((CheckBoxCell)AlteredGameFileModsSource.Rows.RealizeCell(AlteredGameFileModsSource.Columns[0], 0, i))
                    .Value = false;
            }
        }
    }

    private bool CanToggleOverridesExecute() => Mods != null && Mods.Any();
}
