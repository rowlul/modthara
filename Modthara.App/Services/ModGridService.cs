using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;

using Humanizer;

using Modthara.App.Extensions;
using Modthara.App.ViewModels;
using Modthara.Essentials.Packaging;
using Modthara.Lari;

namespace Modthara.App.Services;

/// <inheritdoc />
public class ModGridService : IModGridService
{
    private readonly IModsService _modsService;
    private readonly IModSettingsService _modSettingsService;

    private readonly List<ModPackageViewModel> _viewModels = [];
    public IEnumerable<ModPackageViewModel> ViewModels => _viewModels;

    public ModGridService(IModsService modsService, IModSettingsService modSettingsService)
    {
        _modsService = modsService;
        _modSettingsService = modSettingsService;
    }

    public void CreateViewModels()
    {
        foreach (var pkg in _modsService.ModPackages)
        {
            bool isInOrder = false;

            foreach (var meta in _modSettingsService.ModSettings.Mods)
            {
                if (pkg.Uuid.Value == meta.Uuid.Value)
                {
                    isInOrder = true;
                }
            }

            if (!isInOrder)
            {
                _viewModels.Add(new ModPackageViewModel(pkg, _modsService));
            }
        }
    }

    public FlatTreeDataGridSource<ModPackageViewModel> CreateSource(
        Func<ModPackageViewModel, bool>? filterPredicate = null)
    {
        var source =
            new FlatTreeDataGridSource<ModPackageViewModel>(filterPredicate != null
                ? ViewModels.Where(filterPredicate)
                : ViewModels)
            {
                Columns =
                {
                    new TemplateColumn<ModPackageViewModel>(string.Empty, "ToggleModCell",
                        options: new TemplateColumnOptions<ModPackageViewModel> { CanUserResizeColumn = false }),
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

    public IEnumerable<ModPackageViewModel>? FilterMods(string? query,
        Func<ModPackageViewModel, bool>? filterPredicate = null)
    {
        if (query == null)
        {
            return null;
        }

        if (query == string.Empty)
        {
            return filterPredicate != null ? ViewModels.Where(filterPredicate) : ViewModels;
        }

        if (query.IsWhiteSpace())
        {
            return null;
        }

        // TODO: introduce input debouncing
        var filtered = (filterPredicate != null ? ViewModels.Where(filterPredicate) : ViewModels)
            .Where(x => x.Name.Contains(query, StringComparison.OrdinalIgnoreCase));
        return filtered;
    }

    public void EnableMods(Func<ModPackageViewModel, bool>? filterPredicate = null)
    {
        foreach (var mod in filterPredicate != null ? ViewModels.Where(filterPredicate) : ViewModels)
        {
            if (!mod.IsEnabled)
            {
                mod.Enable();
            }
        }
    }

    public void DisableMods(Func<ModPackageViewModel, bool>? filterPredicate = null)
    {
        foreach (var mod in filterPredicate != null ? ViewModels.Where(filterPredicate) : ViewModels)
        {
            if (mod.IsEnabled)
            {
                mod.Disable();
            }
        }
    }
}
