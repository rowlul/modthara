using Avalonia.Controls;

using Modthara.App.ViewModels;

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
    /// Creates view models from loaded mod packages and sets the <see cref="ViewModels"/> property.
    /// </summary>
    void CreateViewModels();

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
    /// <param name="filterPredicate">
    /// Predicate to filter items by.
    /// </param>
    /// <returns>
    /// List of filtered items.
    /// </returns>
    IEnumerable<ModPackageViewModel>? FilterMods(string? query,
        Func<ModPackageViewModel, bool>? filterPredicate = null);

    void EnableMods(Func<ModPackageViewModel, bool>? filterPredicate = null);

    void DisableMods(Func<ModPackageViewModel, bool>? filterPredicate = null);
}
