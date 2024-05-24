using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Nodes;

using Modthara.Lari;

namespace Modthara.Essentials.Packaging;

public interface IOrderManager
{
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
    /// Loads json order into mod settings.
    /// </summary>
    /// <param name="rootElement">
    /// Root element that contains Order array.
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
    ModSettings LoadJsonOrder(JsonElement rootElement, IReadOnlyList<ModPackage> modPackages);

    /// <summary>
    /// Saves mod settings as a json order.
    /// </summary>
    /// <param name="path">
    /// Path to mod order file.
    /// </param>
    /// <param name="modSettings">
    /// Instance of <see cref="ModSettings"/>.
    /// </param>
    Task SaveJsonOrderAsync(string path, ModSettings modSettings);

    /// <summary>
    /// Creates json order from mod settings.
    /// </summary>
    /// <param name="modSettings">
    /// Instance of <see cref="ModSettings"/>.
    /// </param>
    /// <returns>
    /// Json object with Order array that has entries with UUID and Name.
    /// </returns>
    JsonObject CreateJsonOrderFromModSettings(ModSettings modSettings);

    /// <summary>
    /// Extracts json order and loads it into mod settings.
    /// </summary>
    /// <param name="zipArchive">
    /// Zip archive to extract.
    /// </param>
    /// <param name="modPackages">
    /// List of available mod packages.
    /// </param>
    /// <param name="orderName">
    /// Order name to find if it is known.
    /// </param>
    /// <returns>
    /// Instance of <see cref="ModSettings"/> if order was found, false if order was not found.
    /// </returns>
    ValueTask<ModSettings?> ExtractJsonOrderAsync(ZipArchive zipArchive, IReadOnlyList<ModPackage> modPackages,
        string? orderName = null);
}
