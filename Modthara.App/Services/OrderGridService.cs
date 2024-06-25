using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;

using Humanizer;

using Modthara.App.Extensions;
using Modthara.App.ViewModels;
using Modthara.Essentials.Packaging;
using Modthara.Lari;

namespace Modthara.App.Services;

public class OrderGridService : IOrderGridService
{
    private readonly IModsService _modsService;
    private readonly IModSettingsService _modSettingsService;

    private readonly List<ModPackageViewModel> _viewModels = [];
    public IEnumerable<ModPackageViewModel> ViewModels => _viewModels;

    public OrderGridService(IModsService modsService, IModSettingsService modSettingsService)
    {
        _modsService = modsService;
        _modSettingsService = modSettingsService;
    }

    public void CreateViewModels(out IEnumerable<ModMetadata> missingMods)
    {
        List<ModMetadata> missingModsList = [];

        foreach (var meta in _modSettingsService.ModSettings.Mods)
        {
            // TODO: unhardcode ignored mods (GustavDev)
            if (meta is
                {
                    FolderName: "GustavDev", Name: "GustavDev", Uuid.Value: "28ac9ce2-2aba-8cda-b3b5-6e922f71b6b8"
                })
            {
                continue;
            }

            bool isInModsFolder = false;

            foreach (var pkg in _modsService.ModPackages)
            {
                if (meta.Uuid.Value == pkg.Uuid.Value)
                {
                    _viewModels.Add(new ModPackageViewModel(pkg, _modsService));
                    isInModsFolder = true;
                    break;
                }
            }

            if (!isInModsFolder)
            {
                missingModsList.Add(meta);
            }
        }

        missingMods = missingModsList;
    }

    public FlatTreeDataGridSource<ModPackageViewModel> CreateSource()
    {
        var source =
            new FlatTreeDataGridSource<ModPackageViewModel>(ViewModels)
            {
                Columns =
                {
                    new CheckBoxColumn<ModPackageViewModel>(string.Empty,
                        getter: x => x.IsEnabled,
                        setter: ToggleMod,
                        options: new CheckBoxColumnOptions<ModPackageViewModel> { CanUserResizeColumn = false }),
                    new TextColumn<ModPackageViewModel, string>("Name",
                        getter: x => x.Name,
                        options: new TextColumnOptions<ModPackageViewModel> { IsTextSearchEnabled = true }),
                    new TextColumn<ModPackageViewModel, LariVersion>("Version", getter: x => x.Version),
                    new TextColumn<ModPackageViewModel, string>("Author",
                        getter: x => x.Author),
                    new TextColumn<ModPackageViewModel, string>("Last Modified",
                        getter: x => x.LastModified.ToOrdinalWords(GrammaticalCase.Nominative))
                }
            };

        source.RowSelection!.SingleSelect = false;

        return source;
    }

    public IEnumerable<ModPackageViewModel>? FilterMods(string? query)
    {
        if (query == null)
        {
            return null;
        }

        if (query == string.Empty)
        {
            return ViewModels;
        }

        if (query.IsWhiteSpace())
        {
            return null;
        }

        // TODO: introduce input debouncing
        var filtered = ViewModels
            .Where(x => x.Name.Contains(query, StringComparison.OrdinalIgnoreCase));
        return filtered;
    }

    public void ToggleMod(ModPackageViewModel source, bool newValue)
    {
        throw new NotImplementedException();
    }

    public void ToggleMods(FlatTreeDataGridSource<ModPackageViewModel> source, bool newValue)
    {
        throw new NotImplementedException();
    }
}
