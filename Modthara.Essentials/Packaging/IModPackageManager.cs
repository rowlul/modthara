namespace Modthara.Essentials.Packaging;

/// <summary>
/// Represents a service that handles packages.
/// </summary>
public interface IModPackageManager
{
    /// <summary>
    /// Reads package from PAK file.
    /// </summary>
    /// <param name="path">
    /// Path to package.
    /// </param>
    /// <returns>
    /// Instance of <see cref="ModPackage"/>.
    /// </returns>
    ValueTask<ModPackage> ReadModPackageAsync(string path);

    /// <summary>
    /// Reads packages from a directory of PAK files.
    /// </summary>
    /// <param name="path">
    /// Path to directory.
    /// </param>
    /// <param name="onException">
    /// Method that is executed whenever <see cref="IAsyncEnumerator{T}"/> throws an exception. If null, rethrows the
    /// exception.
    /// </param>
    /// <param name="packageReadCallback">
    /// Callback to current index and item of <see cref="IAsyncEnumerator{T}"/>.
    /// </param>
    /// <returns>
    /// List of packages.
    /// </returns>
    IAsyncEnumerable<ModPackage> ReadModPackagesAsync(string path, Action<Exception>? onException = null,
        PackageReadCallback? packageReadCallback = null);

    /// <summary>
    /// Counts packages in specified path.
    /// </summary>
    /// <param name="path">
    /// Path to directory with packages.
    /// </param>
    /// <returns>
    /// Package count.
    /// </returns>
    int CountModPackages(string path);
}
