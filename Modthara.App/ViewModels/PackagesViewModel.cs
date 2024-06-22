using System.Diagnostics;

using Avalonia.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Modthara.App.Services;
using Modthara.Essentials.Packaging;

namespace Modthara.App.ViewModels;

public partial class PackagesViewModel : ViewModelBase
{
    private readonly IModsService _modsService;
    private readonly IModSettingsService _modSettingsService;
    private readonly IModGridService _modGridService;

    [ObservableProperty]
    private FlatTreeDataGridSource<ModPackageViewModel>? _replacerModsSource;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ToggleReplacerModsCommand))]
    private bool _canToggleReplacerMods;

    [ObservableProperty]
    private bool _areReplacerModsEnabled;

    [ObservableProperty]
    private string _replacerModsSearchText;

    [ObservableProperty]
    private FlatTreeDataGridSource<ModPackageViewModel>? _standaloneModsSource;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ToggleStandaloneModsCommand))]
    private bool _canToggleStandaloneMods;

    [ObservableProperty]
    private bool _areStandaloneModsEnabled;

    [ObservableProperty]
    private string _standaloneModsSearchText;

    public PackagesViewModel(
        IModsService modsService,
        IModSettingsService modSettingsService,
        IModGridService modGridService)
    {
        _modsService = modsService;
        _modSettingsService = modSettingsService;
        _modGridService = modGridService;
    }

    public async Task InitializeViewModel()
    {
        await _modsService.LoadModPackagesAsync();

        List<Task> tasks = [InitializeReplacerModsSource(), InitializeStandaloneModsSource()];
        await Task.WhenAll(tasks);

        CanToggleReplacerMods = ReplacerModsSource != null && ReplacerModsSource.Items.Any();

        CanToggleStandaloneMods = StandaloneModsSource != null && StandaloneModsSource.Items.Any();

        await _modSettingsService.LoadModSettingsAsync();
    }

    private Task InitializeReplacerModsSource() => Task.Run(() =>
    {
        ReplacerModsSource = _modGridService.CreateSource(m => m.HasOverridesExclusively);
        AreReplacerModsEnabled = _modGridService.AnyEnabledMods(ReplacerModsSource.Items);
    });

    private Task InitializeStandaloneModsSource() => Task.Run(() =>
    {
        StandaloneModsSource = _modGridService.CreateSource(m => m.HasAnyStandaloneFiles);
        AreStandaloneModsEnabled = _modGridService.AnyEnabledMods(StandaloneModsSource.Items);
    });

    [RelayCommand(CanExecute = nameof(CanToggleReplacerMods))]
    private void ToggleReplacerMods()
    {
        Debug.Assert(ReplacerModsSource != null, nameof(ReplacerModsSource) + " != null");
        _modGridService.ToggleMods(ReplacerModsSource, AreReplacerModsEnabled);
    }

    [RelayCommand(CanExecute = nameof(CanToggleStandaloneMods))]
    private void ToggleStandaloneMods()
    {
        Debug.Assert(StandaloneModsSource != null, nameof(StandaloneModsSource) + " != null");
        _modGridService.ToggleMods(StandaloneModsSource, AreStandaloneModsEnabled);
    }

    private void OnSearchTextChanged(FlatTreeDataGridSource<ModPackageViewModel>? source,
        Func<ModPackageViewModel, bool> fallbackPredicate, string? query)
    {
        Debug.Assert(source != null, nameof(source) + " != null");

        var filtered = _modGridService.FilterMods(query, fallbackPredicate);
        if (filtered != null)
        {
            source.Items = filtered;
        }
    }

    partial void OnReplacerModsSearchTextChanged(string? value) =>
        OnSearchTextChanged(ReplacerModsSource, m => m.HasOverridesExclusively, value);

    partial void OnStandaloneModsSearchTextChanged(string? value) =>
        OnSearchTextChanged(StandaloneModsSource, m => m.HasAnyStandaloneFiles, value);
}
