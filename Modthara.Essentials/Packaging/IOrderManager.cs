using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Nodes;

using Modthara.Lari;

using MissingModsList = System.Collections.Generic.IReadOnlyList<Modthara.Lari.ModMetadata>;

namespace Modthara.Essentials.Packaging;

public interface IOrderManager
{
    /// <inheritdoc cref="LoadJsonOrder"/>
    ValueTask<(ModSettings, MissingModsList)> LoadJsonOrderAsync(string path,
        IReadOnlyList<ModPackage> modPackages,
        Action<int, ModMetadata>? onMetadataParsed = null);

    /// <summary>
    /// Loads json order into mod settings.
    /// </summary>
    /// <param name="rootElement">
    /// Root element that contains Order array.
    /// </param>
    /// <param name="modPackages">
    /// List of available mod packages.
    /// </param>
    /// <param name="onMetadataParsed">
    /// Callback to current index and mod metadata.
    /// </param>
    /// <returns>
    /// Instance of <see cref="ModSettings"/> and list of mods that are present in the deserialized order but not in <paramref name="modPackages"/>.
    /// </returns>
    (ModSettings, MissingModsList) LoadJsonOrder(JsonElement rootElement, IReadOnlyList<ModPackage> modPackages,
        Action<int, ModMetadata>? onMetadataParsed = null);

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
    ValueTask<(ModSettings, MissingModsList)?> ExtractJsonOrderAsync(ZipArchive zipArchive,
        IReadOnlyList<ModPackage> modPackages,
        string? orderName = null);
}
