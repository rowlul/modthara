using Modthara.Lari;

namespace Modthara.Essentials.Packaging;

/// <summary>
/// Manages mod settings.
/// </summary>
public interface IModSettingsManager
{
    /// <summary>
    /// Loads mod settings from file.
    /// </summary>
    /// <param name="path">
    /// Path to mod settings file.
    /// </param>
    /// <returns>
    /// An instance of <see cref="ModSettings"/>.
    /// </returns>
    ValueTask<ModSettings> LoadModSettingsAsync(string path);

    /// <summary>
    /// Saves mod settings to LSX file.
    /// </summary>
    /// <param name="path">
    /// Path to mod settings file.
    /// </param>
    /// <param name="modSettings">
    /// Instance of <see cref="ModSettings"/>.
    /// </param>
    Task SaveModSettingsAsync(string path, ModSettings modSettings);

    /// <summary>
    /// Parses JSON mod order to LSX mod settings.
    /// </summary>
    /// <param name="path">
    /// Path to mod order file.
    /// </param>
    /// <param name="modPackages">
    /// List of available mod packages.
    /// </param>
    /// <exception cref="ModNotFoundException">
    /// Thrown if parsed mod is not found in <see cref="modPackages"/>.
    /// </exception>
    /// <returns>
    /// Instance of <see cref="ModSettings"/>.
    /// </returns>
    ValueTask<ModSettings> LoadJsonOrderAsync(string path, IReadOnlyList<ModPackage> modPackages);

    /// <summary>
    /// Exports LSX mod settings to JSON mod order.
    /// </summary>
    /// <param name="path">
    /// Path to mod order file.
    /// </param>
    /// <param name="modSettings">
    /// Instance of <see cref="ModSettings"/>.
    /// </param>
    Task SaveJsonOrderAsync(string path, ModSettings modSettings);
}
