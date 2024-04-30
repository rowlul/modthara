using System.Collections.ObjectModel;
using System.Diagnostics;

using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;

using CommunityToolkit.Mvvm.ComponentModel;

using Humanizer;

using Modthara.Essentials.Packaging;
using Modthara.Lari;

namespace Modthara.App.ViewModels;

public partial class PackagesViewModel : ViewModelBase
{
    private readonly IModsService _modsService;

    [ObservableProperty] private ObservableCollection<ModPackage>? _mods;

    [ObservableProperty] private FlatTreeDataGridSource<ModPackage>? _alteredGameFileModsSource;

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
                (x.Flags & (ModFlags.AltersGameFiles | ModFlags.Enabled)) != ModFlags.None &&
                (x.Flags & ModFlags.HasModFiles) == ModFlags.None))
            {
                Columns =
                {
                    new CheckBoxColumn<ModPackage>(string.Empty,
                        getter: x => x.Flags.HasFlag(ModFlags.Enabled),
                        setter: (x, val) =>
                        {
                            // TODO: actually implement toggling mods in the service
                            if (val)
                            {
                                x.Flags |= ModFlags.Enabled;
                                Debug.Assert((x.Flags & ModFlags.Enabled) == ModFlags.Enabled);
                            }
                            else
                            {
                                x.Flags &= ~ModFlags.Enabled;
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
}
