namespace Modthara.Manager;

/// <summary>
/// Provides general paths.
/// </summary>

public interface IPathProvider
{
    /// <summary>
    ///     Gets the path of the Mods directory in the Local data files.
    /// </summary>
    /// <returns>
    ///     A path <see cref="string"> to the given directory.
    /// </returns>
    string GetModsDirectoryPath();

    /// <summary>
    ///     Gets the path of the modsettings.lsx file in the player profile data.
    /// </summary>
    /// <returns>
    ///     A path <see cref="string"> to the given file.
    /// </returns>
    string GetModSettingsPath();

    /// <summary>
    ///     Gets the path of the Data directory in the game files.
    /// </summary>
    /// <returns>
    ///     A path <see cref="string"> to the given directory.
    /// </returns>
    string GetDataDirectoryPath();

    /// <summary>
    ///     Gets the path of the bin directory in the game files.
    /// </summary>
    /// <returns>
    ///     A path <see cref="string"> to the given directory.
    /// </returns>
    string GetBinDirectoryPath();
}