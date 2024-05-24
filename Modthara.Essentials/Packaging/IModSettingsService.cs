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
    ModSettings? ModSettings { get; }

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
    /// <param name="onException">
    /// Method that is executed whenever enumerator throws an exception. If null, rethrows the exception.
    /// </param>
    /// <param name="onPackageRead">
    /// Callback to current index and mod package.
    /// </param>
    /// <exception cref="ModNotFoundException">
    /// Thrown when no entry in <paramref name="modPackages"/> matches one in <see cref="ModSettings.Mods"/>.
    /// </exception>
    /// <returns>
    /// List of found <see cref="ModPackage"/>.
    /// </returns>
    IEnumerable<ModPackage> GetMods(IReadOnlyList<ModPackage> modPackages,
        Action<int, Exception>? onException = null, Action<int, ModPackage>? onPackageRead = null);
}
