using Modthara.Essentials.Packaging;
using Modthara.Lari.Pak;

namespace Modthara.Essentials.Abstractions;

/// <summary>
/// Represents a service that handles packages.
/// </summary>
public interface IModPackageService
{
    /// <summary>
    /// Reads packages in Mods folder.
    /// </summary>
    /// <param name="path">
    /// Path to directory with packages.
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
    IAsyncEnumerable<ModPackage> ReadPackagesAsync(string path, Action<Exception>? onException = null,
        PackageReadCallback? packageReadCallback = null);

    /// <summary>
    /// Returns package count in <paramref name="path"/>.
    /// </summary>
    /// <param name="path">
    /// Path to directory with packages.
    /// </param>
    /// <returns>
    /// Package count.
    /// </returns>
    int CountPackages(string path);

    /// <summary>
    /// Processes <paramref name="pak"/> and creates a <see cref="ModPackage"/>.
    /// </summary>
    /// <param name="path">
    /// Path to package.
    /// </param>
    /// <returns>
    /// Mod package.
    /// </returns>
    ValueTask<ModPackage> CreateModPackageAsync(string path);

    // TODO: docs docs docs

    PackagedFile? FindMeta(Package pak);

    PackagedFile? FindScriptExtenderConfig(Package pak);

    ModFlags RequiresScriptExtender(Package pak);

    ModFlags HasForceRecompile(Package pak);

    ModFlags AltersGameFiles(Package pak);

    ModFlags HasModFiles(Package pak);
}
