using System.Diagnostics;

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
        var items = filterPredicate != null ? ViewModels.Where(filterPredicate) : ViewModels;

        var source =
            new FlatTreeDataGridSource<ModPackageViewModel>(items)
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

    public IEnumerable<ModPackageViewModel>? FilterMods(string? query,
        Func<ModPackageViewModel, bool>? fallbackPredicate = null)
    {
        if (query == null)
        {
            return null;
        }

        if (query == string.Empty)
        {
            return fallbackPredicate != null ? ViewModels.Where(fallbackPredicate) : ViewModels;
        }

        if (query.IsWhiteSpace())
        {
            return null;
        }

        // TODO: introduce input debouncing
        var filtered = (fallbackPredicate != null ? ViewModels.Where(fallbackPredicate) : ViewModels)
            .Where(x => x.Name.Contains(query, StringComparison.OrdinalIgnoreCase));
        return filtered;
    }

    public bool AnyEnabledMods(IEnumerable<ModPackageViewModel> sourceItems) =>
        sourceItems.Any(x => x.Flags.HasFlag(ModFlags.Enabled));

    public void ToggleMod(ModPackageViewModel mod, bool newValue)
    {
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        // ReSharper disable once RedundantBoolCompare
        switch (mod.Flags & ModFlags.Enabled)
        {
            case ModFlags.None when newValue == true:
                {
                    mod.Enable();
                    Debug.Assert((mod.Flags & ModFlags.Enabled) == ModFlags.Enabled);
                    Debug.Assert(mod.Path.Length > 4 && mod.Path[^4..] == ".pak");
                    break;
                }
            case ModFlags.Enabled when newValue == false:
                {
                    mod.Disable();
                    Debug.Assert((mod.Flags & ModFlags.Enabled) == ModFlags.None);
                    Debug.Assert(mod.Path.Length > 8 && mod.Path[^8..] == ".pak.off");
                    break;
                }
        }
    }

    public void ToggleMods(FlatTreeDataGridSource<ModPackageViewModel> source, bool newValue,
        Func<ModPackageViewModel, bool>? filterPredicate = null)
    {
        var mods = filterPredicate != null ? ViewModels.Where(filterPredicate).ToList() : ViewModels.ToList();

        foreach (var mod in mods)
        {
            ToggleMod(mod, newValue);
        }

        source.Items = mods;
    }
}
