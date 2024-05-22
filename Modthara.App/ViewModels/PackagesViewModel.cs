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
    private FlatTreeDataGridSource<ModPackage>? _replacerModsSource;

    [ObservableProperty]
    private bool _areReplacerModsEnabled;

    [ObservableProperty]
    private string _replacerModsSearchText;

    [ObservableProperty]
    private FlatTreeDataGridSource<ModPackage>? _standaloneModsSource;

    [ObservableProperty]
    private bool _areStandaloneModsEnabled;

    [ObservableProperty]
    private string _standaloneModsSearchText;

    public PackagesViewModel(IModsService modsService)
    {
        _modsService = modsService;
    }

    public void InitializeViewModel()
    {
        Mods = GetMods();

        ReplacerModsSource = CreateReplacerModsSource();
        AreReplacerModsEnabled =
            ReplacerModsSource.Items.Any(x => x.Flags.HasFlag(ModFlags.Enabled));

        StandaloneModsSource = CreateStandaloneModsSource();
        AreStandaloneModsEnabled =
            StandaloneModsSource.Items.Any(x => x.Flags.HasFlag(ModFlags.Enabled));
    }

    private ObservableCollection<ModPackage> GetMods() => new(_modsService.ModPackages);

    private FlatTreeDataGridSource<ModPackage> CreateReplacerModsSource()
    {
        Debug.Assert(Mods != null, nameof(Mods) + " != null");

        var source =
            new FlatTreeDataGridSource<ModPackage>(Mods.Where(HasReplacerMods))
            {
                Columns =
                {
                    new CheckBoxColumn<ModPackage>(string.Empty,
                        getter: x => x.Flags.HasFlag(ModFlags.Enabled),
                        setter: ToggleMod,
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

    private FlatTreeDataGridSource<ModPackage> CreateStandaloneModsSource()
    {
        Debug.Assert(Mods != null, nameof(Mods) + " != null");

        var source =
            new FlatTreeDataGridSource<ModPackage>(Mods.Where(HasStandaloneMods))
            {
                Columns =
                {
                    new CheckBoxColumn<ModPackage>(string.Empty,
                        getter: x => x.Flags.HasFlag(ModFlags.Enabled),
                        setter: ToggleMod,
                        options: new CheckBoxColumnOptions<ModPackage> { CanUserResizeColumn = false }),
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

    private void ToggleMod(ModPackage modPackage, bool newValue)
    {
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        // ReSharper disable once RedundantBoolCompare
        switch (modPackage.Flags & ModFlags.Enabled)
        {
            case ModFlags.None when newValue == true:
                {
                    _modsService.EnableModPackage(modPackage);
                    Debug.Assert((modPackage.Flags & ModFlags.Enabled) == ModFlags.Enabled);
                    Debug.Assert(modPackage.Path.Length > 4 && modPackage.Path[^4..] == ".pak");
                    break;
                }
            case ModFlags.Enabled when newValue == false:
                {
                    _modsService.DisableModPackage(modPackage);
                    Debug.Assert((modPackage.Flags & ModFlags.Enabled) == ModFlags.None);
                    Debug.Assert(modPackage.Path.Length > 8 && modPackage.Path[^8..] == ".pak.off");
                    break;
                }
        }
    }

    private void ToggleMods(FlatTreeDataGridSource<ModPackage>? source, bool toggleValue)
    {
        Debug.Assert(source != null, nameof(source) + " != null");

        if (toggleValue)
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

    [RelayCommand(CanExecute = nameof(CanToggleReplacerMods))]
    private void ToggleReplacerMods() => ToggleMods(ReplacerModsSource, AreReplacerModsEnabled);

    private bool CanToggleReplacerMods() => ReplacerModsSource != null && ReplacerModsSource.Items.Any();

    [RelayCommand(CanExecute = nameof(CanToggleStandaloneMods))]
    private void ToggleStandaloneMods() => ToggleMods(StandaloneModsSource, AreStandaloneModsEnabled);

    private bool CanToggleStandaloneMods() => ReplacerModsSource != null && ReplacerModsSource.Items.Any();

    private void OnSearchTextChanged(FlatTreeDataGridSource<ModPackage>? source,
        Func<ModPackage, bool> unfilteredPredicate, string? value)
    {
        Debug.Assert(source != null, nameof(source) + " != null");

        var filtered = FilterModsByKeyword(unfilteredPredicate, value);
        if (filtered != null)
        {
            source.Items = filtered;
        }
    }

    partial void OnReplacerModsSearchTextChanged(string? value) =>
        OnSearchTextChanged(ReplacerModsSource, HasReplacerMods, value);

    partial void OnStandaloneModsSearchTextChanged(string? value) =>
        OnSearchTextChanged(StandaloneModsSource, HasStandaloneMods, value);

    private IEnumerable<ModPackage>? FilterModsByKeyword(Func<ModPackage, bool> unfilteredPredicate, string? query)
    {
        Debug.Assert(Mods != null, nameof(Mods) + " != null");

        if (query == null)
        {
            return null;
        }

        if (query == string.Empty)
        {
            return Mods.Where(unfilteredPredicate);
        }

        if (IsWhiteSpace(query))
        {
            return null;
        }

        // TODO: introduce input debouncing
        var filtered = Mods
            .Where(unfilteredPredicate)
            .Where(x => x.Name.Contains(query, StringComparison.OrdinalIgnoreCase));
        return filtered;

        bool IsWhiteSpace(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (!char.IsWhiteSpace(s[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }

    private static bool HasReplacerMods(ModPackage modPackage) =>
        modPackage.Flags.HasFlag(ModFlags.AltersGameFiles) &&
        !modPackage.Flags.HasFlag(ModFlags.HasModFiles);

    private static bool HasStandaloneMods(ModPackage modPackage) =>
        modPackage.Flags.HasFlag(ModFlags.HasModFiles);
}
