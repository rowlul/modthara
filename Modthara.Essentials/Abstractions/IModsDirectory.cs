using Modthara.Essentials.Packaging;

namespace Modthara.Essentials.Abstractions;

public interface IModsDirectory
{
    /// <summary>
    /// List of cached packages.
    /// </summary>
    IReadOnlyList<ModPackage> Packages { get; }

    /// <summary>
    /// Loads packages to <see cref="Packages"/> for cached access.
    /// </summary>
    /// <param name="asyncPackageCallback">
    /// Callback for each enumerated package.
    /// </param>
    /// <param name="onException">
    /// Method that is executed whenever <see cref="IAsyncEnumerator{T}"/> in <see cref="ReadPackagesAsync"/> throws
    /// an exception. If null, rethrows the exception.
    /// </param>
    Task LoadPackagesAsync(Action<Exception>? onException = null, PackageReadCallback? asyncPackageCallback = null);

    /// <summary>
    /// Counts packages in mods directory.
    /// </summary>
    /// <returns>
    /// Package count.
    /// </returns>
    int CountPackages();
}
