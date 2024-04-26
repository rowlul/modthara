using Modthara.Lari;

namespace Modthara.Essentials.Packaging;

/// <summary>
/// Represents a service that handles mod orders.
/// </summary>
public interface IModSettingsManager
{
    /// <summary>
    /// Reads mod settings file.
    /// </summary>
    /// <param name="path">
    /// Path to mod settings file.
    /// </param>
    /// <returns>
    /// An instance of <see cref="ModSettings"/>.
    /// </returns>
    Task<ModSettings> ReadModSettingsAsync(string path);

    /// <summary>
    /// Saves mod settings to file.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="modSettings"></param>
    /// <returns></returns>
    Task SaveModSettingsAsync(string path, ModSettings modSettings);
}
