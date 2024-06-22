using Modthara.Lari;

namespace Modthara.Essentials.Packaging;

/// <summary>
/// Manages mod settings.
/// </summary>
public interface IModSettingsService
{
    /// <summary>
    /// Current mod settings.
    /// </summary>
    ModSettings ModSettings { get; }

    /// <summary>
    /// Loads mod settings from file to <see cref="ModSettings"/>.
    /// </summary>
    Task LoadModSettingsAsync();

    /// <summary>
    /// Saves current mod settings to LSX file.
    /// </summary>
    Task SaveModSettingsAsync();

    /// <summary>
    /// Enumerates over mods in <see cref="ModSettings"/> to find a respective mod for each
    /// <paramref name="modPackages"/> entry.
    /// </summary>
    /// <param name="modPackages">
    /// List of loaded mod packages.
    /// </param>
    /// <param name="missingMods">
    /// List of mods found in <see cref="ModSettings.Mods"/> but not <paramref name="modPackages"/>.
    /// </param>
    /// <param name="onPackageRead">
    /// Callback to current index and mod package.
    /// </param>
    /// <returns>
    /// List of found <see cref="ModPackage"/>.
    /// </returns>
    IEnumerable<ModPackage> GetMods(IReadOnlyList<ModPackage> modPackages,
        out IEnumerable<ModMetadata> missingMods, Action<int, ModPackage>? onPackageRead = null);
}
