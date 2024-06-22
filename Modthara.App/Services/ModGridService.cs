using System.Diagnostics;

using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;

using Humanizer;

using Modthara.App.ViewModels;
using Modthara.Essentials.Packaging;
using Modthara.Lari;

namespace Modthara.App.Services;

/// <inheritdoc />
public class ModGridService : IModGridService
{
    private readonly IModsService _modsService;

    private readonly List<ModPackageViewModel> _viewModels = [];
    public IEnumerable<ModPackageViewModel> ViewModels => _viewModels;

    public ModGridService(IModsService modsService)
    {
        _modsService = modsService;
    }

    public void CreateViewModels()
    {
        foreach (var modPackage in _modsService.ModPackages)
        {
            _viewModels.Add(new ModPackageViewModel(modPackage, _modsService));
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
                        getter: x => x.Flags.HasFlag(ModFlags.Enabled),
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
        Func<ModPackageViewModel, bool> fallbackPredicate)
    {
        if (query == null)
        {
            return null;
        }

        if (query == string.Empty)
        {
            return ViewModels.Where(fallbackPredicate);
        }

        if (IsWhiteSpace(query))
        {
            return null;
        }

        // TODO: introduce input debouncing
        var filtered = ViewModels
            .Where(fallbackPredicate)
            .Where(x => x.Name.Contains(query, StringComparison.OrdinalIgnoreCase));
        return filtered;

        bool IsWhiteSpace(string s)
        {
            foreach (var c in s)
            {
                if (!char.IsWhiteSpace(c))
                {
                    return false;
                }
            }

            return true;
        }
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

    public void ToggleMods(FlatTreeDataGridSource<ModPackageViewModel> source, bool newValue)
    {
        if (newValue)
        {
            for (int i = 0; i < source.Rows.Count; i++)
            {
                ((CheckBoxCell)source.Rows.RealizeCell(source.Columns[0], 0, i))
                    .Value = true;
            }
        }
        else
        {
            for (int i = 0; i < source.Rows.Count; i++)
            {
                ((CheckBoxCell)source.Rows.RealizeCell(source.Columns[0], 0, i))
                    .Value = false;
            }
        }
    }
}
