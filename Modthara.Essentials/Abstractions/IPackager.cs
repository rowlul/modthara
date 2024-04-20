using System.Collections.Concurrent;

using Modthara.Essentials.Packaging;

namespace Modthara.Essentials.Abstractions;

/// <summary>
/// Represents a service that handles packages.
/// </summary>
public interface IPackager
{
    /// <summary>
    /// Stores cached packages.
    /// </summary>
    IEnumerable<ModPackage> Cache { get; }

    /// <summary>
    /// Reads packages in Mods folder.
    /// </summary>
    /// <returns>
    /// List of packages.
    /// </returns>
    public IEnumerable<ModPackage> ReadPackages();

    /// <inheritdoc cref="ReadPackages"/>
    public IAsyncEnumerable<ModPackage> ReadPackagesAsync();

    /// <summary>
    /// Loads packages to <see cref="Cache"/> for cached access.
    /// </summary>
    /// <param name="asyncPackageCallback">Callback for each enumerated package.</param>
    /// <returns></returns>
    public Task LoadPackagesToCacheAsync(AsyncPackageCallback? asyncPackageCallback = null);

    /// <summary>
    /// Counts mods in Mods folder.
    /// </summary>
    /// <returns>
    /// Amount of installed mods.
    /// </returns>
    public int Count();

    /// <inheritdoc cref="Count"/>
    public ValueTask<int> CountAsync();
}
