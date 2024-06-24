using System.Diagnostics;

using Avalonia.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Modthara.App.Services;
using Modthara.Essentials.Packaging;
using Modthara.Lari;

namespace Modthara.App.ViewModels;

using Source = FlatTreeDataGridSource<ModPackageViewModel>;

public partial class PackagesViewModel : ViewModelBase
{
    private readonly IModsService _modsService;
    private readonly IModSettingsService _modSettingsService;
    private readonly IModGridService _modGridService;
    private readonly IOrderGridService _orderGridService;

    [ObservableProperty]
    private bool _isViewReady;

    #region Replacer Mods

    [ObservableProperty]
    private Source? _replacerModsSource;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ToggleReplacerModsCommand))]
    private bool _canToggleReplacerMods;

    [ObservableProperty]
    private bool _areReplacerModsEnabled;

    [ObservableProperty]
    private string? _replacerModsSearchText;

    [RelayCommand(CanExecute = nameof(CanToggleReplacerMods))]
    private void ToggleReplacerMods()
    {
        Debug.Assert(ReplacerModsSource != null, nameof(ReplacerModsSource) + " != null");
        _modGridService.ToggleMods(ReplacerModsSource, AreReplacerModsEnabled, m => m.HasOverridesExclusively);
        ReplacerModsSearchText = null;
    }

    #endregion

    #region Standalone Mods

    [ObservableProperty]
    private Source? _standaloneModsSource;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ToggleStandaloneModsCommand))]
    private bool _canToggleStandaloneMods;

    [ObservableProperty]
    private bool _areStandaloneModsEnabled;

    [ObservableProperty]
    private string? _standaloneModsSearchText;

    [RelayCommand(CanExecute = nameof(CanToggleStandaloneMods))]
    private void ToggleStandaloneMods()
    {
        Debug.Assert(StandaloneModsSource != null, nameof(StandaloneModsSource) + " != null");
        _modGridService.ToggleMods(StandaloneModsSource, AreStandaloneModsEnabled, m => m.HasAnyStandaloneFiles);
        StandaloneModsSearchText = null;
    }

    #endregion

    #region Order Mods

    [ObservableProperty]
    private Source? _orderModsSource;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ToggleOrderModsCommand))]
    private bool _canToggleOrderMods;

    [ObservableProperty]
    private bool _areOrderModsEnabled;

    [ObservableProperty]
    private string? _orderModsSearchText;

    [RelayCommand(CanExecute = nameof(CanToggleOrderMods))]
    private void ToggleOrderMods()
    {
        Debug.Assert(OrderModsSource != null, nameof(OrderModsSource) + " != null");
        _orderGridService.ToggleMods(OrderModsSource, AreOrderModsEnabled);
    }

    #endregion

    public PackagesViewModel(
        IModsService modsService,
        IModSettingsService modSettingsService,
        IModGridService modGridService,
        IOrderGridService orderGridService)
    {
        _modsService = modsService;
        _modSettingsService = modSettingsService;
        _modGridService = modGridService;
        _orderGridService = orderGridService;
    }

    public async Task InitializeViewModel()
    {
        await Task.WhenAll(_modsService.LoadModPackagesAsync(), _modSettingsService.LoadModSettingsAsync());

        await Task.Run(() => _modGridService.CreateViewModels());
        await Task.Run(() =>
        {
            IEnumerable<ModMetadata> missingMods;
            _orderGridService.CreateViewModels(out missingMods);
        });

        await Task.WhenAll(
                Task.Run(() =>
                {
                    ReplacerModsSource = _modGridService.CreateSource(m => m.HasOverridesExclusively);
                    AreReplacerModsEnabled = _modGridService.AnyEnabledMods(ReplacerModsSource.Items);
                }),
                Task.Run(() =>
                {
                    StandaloneModsSource = _modGridService.CreateSource(m => m.HasAnyStandaloneFiles);
                    AreStandaloneModsEnabled = _modGridService.AnyEnabledMods(StandaloneModsSource.Items);
                }),
                Task.Run(() =>
                {
                    OrderModsSource = _orderGridService.CreateSource();
                    AreOrderModsEnabled = OrderModsSource.Items.Any();
                }))
            .ContinueWith(_ =>
            {
                IsViewReady = true;
            });

        CanToggleReplacerMods = IsSourceNotEmpty(ReplacerModsSource);
        CanToggleStandaloneMods = IsSourceNotEmpty(StandaloneModsSource);
        CanToggleOrderMods = IsSourceNotEmpty(OrderModsSource);
    }

    private void OnModsSearchTextChanged(Source? source,
        Func<ModPackageViewModel, bool> fallbackPredicate, string? query)
    {
        Debug.Assert(source != null, nameof(source) + " != null");

        var filtered = _modGridService.FilterMods(query, fallbackPredicate);
        if (filtered != null)
        {
            source.Items = filtered;
        }
    }

    private void OnOrderSearchTextChanged(Source? source, string? query)
    {
        Debug.Assert(source != null, nameof(source) + " != null");

        var filtered = _orderGridService.FilterMods(query);
        if (filtered != null)
        {
            source.Items = filtered;
        }
    }

    private bool IsSourceNotEmpty(Source? source) => source != null && source.Items.Any();

    partial void OnReplacerModsSearchTextChanged(string? value) =>
        OnModsSearchTextChanged(ReplacerModsSource, m => m.HasOverridesExclusively, value);

    partial void OnStandaloneModsSearchTextChanged(string? value) =>
        OnModsSearchTextChanged(StandaloneModsSource, m => m.HasAnyStandaloneFiles, value);

    partial void OnOrderModsSearchTextChanged(string? value) =>
        OnOrderSearchTextChanged(OrderModsSource, value);
}
