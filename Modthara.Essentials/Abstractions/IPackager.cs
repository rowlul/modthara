using Modthara.Essentials.Packaging;

namespace Modthara.Essentials.Abstractions;

/// <summary>
/// Represents a service that handles packages.
/// </summary>
public interface IPackager
{
    /// <summary>
    /// Reads packages in Mods folder.
    /// </summary>
    /// <returns>
    /// List of packages.
    /// </returns>
    public IEnumerable<ModPackage> ReadPackages();

    /// <inheritdoc cref="ReadPackages"/>
    public IAsyncEnumerable<ModPackage> ReadPackagesAsync();
}
