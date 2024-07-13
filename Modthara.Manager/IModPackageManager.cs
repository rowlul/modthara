namespace Modthara.Manager;

/// <summary>
/// Manages PAK files.
/// </summary>
public interface IModPackageManager
{
    /// <summary>
    /// Reads package from PAK file.
    /// </summary>
    /// <param name="path">
    /// Path to package.
    /// </param>
    /// <param name="leaveOpen">
    /// Determines whether to leave underlying <see cref="FileStream"/> open or not.
    /// </param>
    /// <returns>
    /// Instance of <see cref="ModPackage"/>.
    /// </returns>
    ValueTask<ModPackage> ReadModPackageAsync(string path, bool leaveOpen = false);

    /// <summary>
    /// Reads package from stream.
    /// </summary>
    /// <param name="stream">
    /// Stream of the package.
    /// </param>
    /// <param name="path">
    /// Path to package.
    /// </param>
    /// <param name="leaveOpen">
    /// Determines whether to leave underlying <see cref="FileStream"/> open or not.
    /// </param>
    /// <returns>
    /// Instance of <see cref="ModPackage"/>.
    /// </returns>
    ValueTask<ModPackage> ReadModPackageAsync(Stream stream, string path, bool leaveOpen = false);

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
    /// <param name="onPackageRead">
    /// Callback to current index and item of <see cref="IAsyncEnumerator{T}"/>.
    /// </param>
    /// <returns>
    /// List of packages.
    /// </returns>
    IAsyncEnumerable<ModPackage> ReadModPackagesAsync(string path, Action<int, Exception>? onException = null,
        Action<int, ModPackage>? onPackageRead = null);
}
