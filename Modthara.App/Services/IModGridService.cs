using Avalonia.Controls;

using Modthara.App.ViewModels;
using Modthara.Essentials.Packaging;

namespace Modthara.App.Services;

/// <summary>
/// Provides some boilerplate methods for handling <see cref="FlatTreeDataGridSource{TModel}"/> where <c>TModel</c> is
/// <see cref="ModPackageViewModel"/>.
/// </summary>
public interface IModGridService
{
    /// <summary>
    /// Cache of items accessed by any source.
    /// </summary>
    IEnumerable<ModPackageViewModel> ViewModels { get; }

    /// <summary>
    /// Create view models from loaded mod packages.
    /// </summary>
    /// <returns>
    /// View models that represent loaded mod packages.
    /// </returns>
    IEnumerable<ModPackageViewModel> CreateViewModels();

    /// <summary>
    /// Creates a source with optional filtering.
    /// </summary>
    /// <param name="filterPredicate">
    /// Predicate to filter items by.
    /// </param>
    /// <returns>
    /// Source with filtered items.
    /// </returns>
    FlatTreeDataGridSource<ModPackageViewModel> CreateSource(
        Func<ModPackageViewModel, bool>? filterPredicate = null);

    /// <summary>
    /// Filters mods by <paramref name="query"/> input.
    /// </summary>
    /// <param name="query">
    /// Substring to search for.
    /// </param>
    /// <param name="fallbackPredicate">
    /// Predicate to filter items by when <paramref name="query"/> is null, empty, or whitespace.
    /// </param>
    /// <returns>
    /// List of filtered items.
    /// </returns>
    IEnumerable<ModPackageViewModel>? FilterMods(string? query,
        Func<ModPackageViewModel, bool> fallbackPredicate);

    /// <summary>
    /// Determines whether there are any enabled mods in <paramref name="sourceItems"/>.
    /// </summary>
    /// <param name="sourceItems">
    /// List of mods.
    /// </param>
    /// <returns>
    /// True, if there are any mods with <see cref="ModFlags.Enabled"/> flag; false, if there are no such mods.
    /// </returns>
    bool AnyEnabledMods(IEnumerable<ModPackageViewModel> sourceItems);

    /// <summary>
    /// Toggles a mod according to <paramref name="newValue"/>.
    /// </summary>
    /// <param name="mod">
    /// Mod to toggle.
    /// </param>
    /// <param name="newValue">
    /// New value to set.
    /// </param>
    void ToggleMod(ModPackageViewModel mod, bool newValue);

    /// <summary>
    /// Toggles all mods in <paramref name="source"/> Items according to <paramref name="newValue"/>.
    /// </summary>
    /// <param name="source">
    /// Source with items.
    /// </param>
    /// <param name="newValue">
    /// New value to set.
    /// </param>
    void ToggleMods(FlatTreeDataGridSource<ModPackageViewModel> source, bool newValue);
}
