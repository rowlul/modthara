using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;

using Humanizer;

using Modthara.Essentials.Packaging;
using Modthara.Lari;

namespace Modthara.App.TreeDataGrid;

public class ModGridService
{
    private readonly IModsService _modsService;

    public ModGridService(IModsService modsService)
    {
        _modsService = modsService;
    }

    public ITreeDataGridSource<ModPackage> CreateSource(Func<ModPackage, bool> filterPredicate)
    {
        var source =
            new FlatTreeDataGridSource<ModPackage>(_modsService.ModPackages.Where(filterPredicate))
            {
                Columns =
                {
                    new TextColumn<ModPackage, string>("Name",
                        getter: x => x.Name,
                        options: new TextColumnOptions<ModPackage> { IsTextSearchEnabled = true }),
                    new TextColumn<ModPackage, LariVersion>("Version", getter: x => x.Version),
                    new TextColumn<ModPackage, string>("Author",
                        getter: x => x.Author),
                    new TextColumn<ModPackage, string>("Last Modified",
                        getter: x => x.LastModified.Humanize(null, null, null)),
                }
            };

        source.RowSelection!.SingleSelect = false;

        return source;
    }
}
