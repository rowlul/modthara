using Modthara.Lari;

namespace Modthara.Manager;

/// <summary>
/// Manages mod settings.
/// </summary>
public interface IModSettingsService
{
    /// <summary>
    /// Current instance of <see cref="ModSettings"/>.
    /// </summary>
    ModSettings ModSettings { get; }

    /// <summary>
    /// Loads mod settings from a file to <see cref="ModSettings"/>.
    /// </summary>
    Task LoadModSettingsAsync();

    /// <summary>
    /// Saves current mod settings to a file.
    /// </summary>
    Task SaveModSettingsAsync();
}
