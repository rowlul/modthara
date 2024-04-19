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
    public IList<ModPackage> ReadPackages();

    /// <inheritdoc cref="ReadPackages"/>
    public Task<IList<ModPackage>> ReadPackagesAsync();
}
