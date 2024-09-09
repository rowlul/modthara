namespace Modthara.Manager;

/// <summary>
/// Manages mod packages.
/// </summary>
public interface IModPackageManager
{
    /// <summary>
    /// Reads a mod package from the specified file path.
    /// </summary>
    /// <param name="path">
    /// The path to the PAK file.
    /// </param>
    /// <param name="leaveOpen">
    /// If <c>true</c>, leaves the file stream open after reading; otherwise, closes the file stream.
    /// </param>
    /// <returns>
    /// A <see cref="ModPackage"/> with the loaded data.
    /// </returns>
    ValueTask<ModPackage> ReadModPackageAsync(string path, bool leaveOpen = false);

    /// <param name="stream">
    /// The stream to read the mod package from.
    /// </param>
    /// <inheritdoc cref="ReadModPackageAsync(string,bool)"/>
    ValueTask<ModPackage> ReadModPackageAsync(Stream stream, string path, bool leaveOpen = false);

    /// <summary>
    /// Reads mod packages from the specified directory.
    /// </summary>
    /// <param name="path">The path to the directory containing the mod packages.</param>
    /// <param name="onException">
    /// An optional callback invoked when an exception occurs during the reading of a mod package.
    /// </param>
    /// <param name="onPackageRead">
    /// An optional callback invoked when a mod package is successfully read.
    /// </param>
    /// <returns>
    /// An <see cref="IAsyncEnumerable{ModPackage}"/> with the loaded mod packages.
    /// </returns>
    ValueTask<List<ModPackage>> ReadModPackagesAsync(string path, Action<int, Exception>? onException = null,
        Action<int, ModPackage>? onPackageRead = null);
}
