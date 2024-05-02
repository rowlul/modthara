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

    [ObservableProperty]
    private ObservableCollection<ModPackage>? _mods;

    [ObservableProperty]
    private FlatTreeDataGridSource<ModPackage>? _overridesSource;

    [ObservableProperty]
    private bool _areOverridesEnabled;

    [ObservableProperty]
    private string _overridesSearchText;

    public PackagesViewModel(IModsService modsService)
    {
        _modsService = modsService;
    }

    public void InitializeViewModel()
    {
        Mods = GetMods();
        OverridesSource = CreateOverridesSource();
        AreOverridesEnabled =
            OverridesSource.Items.Any(x => (x.Flags & ModFlags.Enabled) == ModFlags.Enabled);
    }

    private ObservableCollection<ModPackage> GetMods() => new(_modsService.ModPackages);

    private FlatTreeDataGridSource<ModPackage> CreateOverridesSource()
    {
        Debug.Assert(Mods != null, nameof(Mods) + " != null");

        var source =
            new FlatTreeDataGridSource<ModPackage>(Mods.Where(HasOverrides))
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
        Debug.Assert(OverridesSource != null, nameof(OverridesSource) + " != null");

        if (AreOverridesEnabled)
        {
            for (int i = 0; i < OverridesSource.Rows.Count; i++)
            {
                ((CheckBoxCell)OverridesSource.Rows.RealizeCell(OverridesSource.Columns[0], 0, i))
                    .Value = true;
            }
        }
        else
        {
            for (int i = 0; i < OverridesSource.Rows.Count; i++)
            {
                ((CheckBoxCell)OverridesSource.Rows.RealizeCell(OverridesSource.Columns[0], 0, i))
                    .Value = false;
            }
        }
    }

    private bool CanToggleOverridesExecute() => Mods != null && Mods.Any();

    partial void OnOverridesSearchTextChanged(string? value)
    {
        Debug.Assert(OverridesSource != null, nameof(OverridesSource) + " != null");
        Debug.Assert(Mods != null, nameof(Mods) + " != null");

        var unfiltered = Mods
            .Where(HasOverrides);

        if (string.IsNullOrWhiteSpace(value))
        {
            OverridesSource.Items = unfiltered;
            return;
        }

        // TODO: introduce input debouncing
        var filtered = unfiltered.Where(x => x.Name.Contains(value, StringComparison.OrdinalIgnoreCase));
        OverridesSource.Items = filtered;
    }

    private static bool HasOverrides(ModPackage modPackage) =>
        (modPackage.Flags & ModFlags.AltersGameFiles) != ModFlags.None &&
        (modPackage.Flags & ModFlags.HasModFiles) == ModFlags.None;
}
