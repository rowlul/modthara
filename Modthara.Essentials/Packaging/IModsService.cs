using System.IO.Compression;

namespace Modthara.Essentials.Packaging;

/// <summary>
/// Represents a Mods directory.
/// </summary>
public interface IModsService
{
    /// <summary>
    /// List of cached packages.
    /// </summary>
    IReadOnlyList<ModPackage> ModPackages { get; }

    /// <summary>
    /// Loads packages to <see cref="ModPackages"/> for cached access.
    /// </summary>
    /// <param name="asyncPackageCallback">
    /// Callback for each enumerated package.
    /// </param>
    /// <param name="onException">
    /// Method to be executed whenever enumerator throws an exception. If null, rethrows the exception.
    /// </param>
    Task LoadModPackagesAsync(Action<Exception>? onException = null, PackageReadCallback? asyncPackageCallback = null);

    /// <summary>
    /// Counts packages in Mods directory.
    /// </summary>
    /// <returns>
    /// Package count.
    /// </returns>
    int CountModPackages();

    /// <summary>
    /// Finds duplicate packages in Mods directory.
    /// </summary>
    /// <returns>
    /// Hash set of duplicate packages.
    /// </returns>
    IReadOnlySet<(ModPackage, ModPackage)> FindDuplicateModPackages();

    /// <summary>
    /// Copies mod package to Mods directory and adds it to <see cref="ModPackages"/>.
    /// </summary>
    /// <param name="modPackage">
    /// Mod package to import.
    /// </param>
    Task ImportModPackageAsync(ModPackage modPackage);

    /// <summary>
    /// Extracts archived mod packages and copies them to Mods directory.
    /// </summary>
    /// <param name="zipArchive">
    /// Zip archive to extract.
    /// </param>
    /// <param name="onException">
    /// Method that is executed whenever <see cref="IEnumerator{T}"/> throws an exception. If null, rethrows the
    /// exception.
    /// </param>
    /// <param name="packageReadCallback">
    /// Callback to current index and item of <see cref="IEnumerator{T}"/>.
    /// </param>
    Task ImportArchivedModPackagesAsync(ZipArchive zipArchive, Action<Exception>? onException = null,
        PackageReadCallback? packageReadCallback = null);

    /// <summary>
    /// Removes '.off' from the file extension and marks <paramref name="modPackage"/> as enabled.
    /// </summary>
    /// <param name="modPackage">
    /// Mod package to enable.
    /// </param>
    void EnableModPackage(ModPackage modPackage);

    /// <summary>
    /// Appends '.off' to the file extension and marks <paramref name="modPackage"/> as enabled.
    /// </summary>
    /// <param name="modPackage">
    /// Mod package to enable.
    /// </param>
    void DisableModPackage(ModPackage modPackage);

    /// <summary>
    /// Deletes package from Mods directory.
    /// </summary>
    /// <param name="modPackage">
    /// Mod package to delete.
    /// </param>
    void DeleteModPackage(ModPackage modPackage);
}
