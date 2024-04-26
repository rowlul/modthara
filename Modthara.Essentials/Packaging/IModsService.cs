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
    /// Reads package from <paramref name="path"/> and copies it to Mods directory.
    /// </summary>
    /// <param name="path">
    /// Path to package.
    /// </param>
    /// <returns>
    /// Imported package.
    /// </returns>
    Task<ModPackage> ImportModPackageAsync(string path);

    /// <summary>
    /// Deletes package from <paramref name="path"/>.
    /// </summary>
    /// <param name="path">
    /// Path to package.
    /// </param>
    void DeleteModPackage(string path);
}
