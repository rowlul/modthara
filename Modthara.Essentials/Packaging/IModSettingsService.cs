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
}
