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
    /// Amount of cached mods.
    /// </summary>
    int CachedModCount { get; }

    /// <summary>
    /// Reads packages in Mods folder.
    /// </summary>
    /// <returns>
    /// List of packages.
    /// </returns>
    public IEnumerable<ModPackage> ReadPackages();

    /// <summary>
    /// Reads packages in Mods folder.
    /// </summary>
    /// <param name="onException">
    /// Method that is executed whenever <see cref="IAsyncEnumerator{T}"/> throws an exception. If null, rethrows the
    /// exception.
    /// </param>
    /// <returns>
    /// List of packages.
    /// </returns>
    public IAsyncEnumerable<ModPackage> ReadPackagesAsync(Func<Exception, Task>? onException = null);

    /// <summary>
    /// Loads packages to <see cref="Cache"/> for cached access.
    /// </summary>
    /// <param name="asyncPackageCallback">
    /// Callback for each enumerated package.
    /// </param>
    /// <param name="onException">
    /// Method that is executed whenever <see cref="IAsyncEnumerator{T}"/> in <see cref="ReadPackagesAsync"/> throws
    /// an exception. If null, rethrows the exception.
    /// </param>
    /// <returns></returns>
    public Task LoadPackagesToCacheAsync(AsyncPackageCallback? asyncPackageCallback = null,
        Func<Exception, Task>? onException = null);

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
