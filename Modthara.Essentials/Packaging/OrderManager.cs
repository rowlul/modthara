using System.IO.Abstractions;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Nodes;

using Modthara.Lari;

namespace Modthara.Essentials.Packaging;

public class OrderManager : IOrderManager
{
    private readonly IFileSystem _fileSystem;

    public OrderManager(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    /// <inheritdoc />
    public async ValueTask<(ModSettings, IReadOnlyList<ModMetadata>)> LoadJsonOrderAsync(string path,
        IReadOnlyList<ModPackage> modPackages,
        Action<int, ModMetadata>? onMetadataParsed = null)
    {
        await using var file = _fileSystem.FileStream.New(path, FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: 4096, useAsync: true);

        var document = await JsonDocument.ParseAsync(file).ConfigureAwait(false);


        var order = await Task
            .Run(() => LoadJsonOrder(document.RootElement, modPackages, onMetadataParsed))
            .ConfigureAwait(false);

        return order;
    }

    /// <inheritdoc />
    public (ModSettings, IReadOnlyList<ModMetadata>) LoadJsonOrder(JsonElement rootElement,
        IReadOnlyList<ModPackage> modPackages,
        Action<int, ModMetadata>? onMetadataParsed = null)
    {
        List<ModMetadata> mods = [];
        List<ModMetadata> missingMods = [];

        int i = 0;
        foreach (var el in rootElement.GetProperty("Order").EnumerateArray())
        {
            i++;

            var uuid = el.GetProperty("UUID").GetString();
            if (string.IsNullOrEmpty(uuid))
            {
                continue;
            }

            var name = el.GetProperty("Name").GetString();
            if (string.IsNullOrEmpty(name))
            {
                continue;
            }

            ModMetadata? modMetadata = null;

            foreach (var m in modPackages)
            {
                if (m.Uuid.Value == uuid && m.Name == name)
                {
                    mods.Add(m);
                }
                else
                {
                    missingMods.Add(m);
                }
            }

            if (modMetadata != null)
            {
                onMetadataParsed?.Invoke(i, modMetadata);
            }
        }

        var modSettings = new ModSettings(mods: mods);
        return (modSettings, missingMods);
    }

    /// <inheritdoc />
    public async Task SaveJsonOrderAsync(string path, ModSettings modSettings)
    {
        var order = await Task.Run(() => CreateJsonOrderFromModSettings(modSettings)).ConfigureAwait(false);

        await _fileSystem.File.WriteAllTextAsync(path,
                order.ToJsonString(new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    AllowTrailingCommas = false
                }))
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public JsonObject CreateJsonOrderFromModSettings(ModSettings modSettings)
    {
        var order = new JsonArray();

        foreach (var modMeta in modSettings.Mods)
        {
            var mod = new JsonObject { { "UUID", modMeta.Uuid.Value }, { "Name", modMeta.Name } };
            order.Add(mod);
        }

        var root = new JsonObject { { "Order", order } };
        return root;
    }

    /// <inheritdoc />
    public async ValueTask<(ModSettings, IReadOnlyList<ModMetadata>)?> ExtractJsonOrderAsync(ZipArchive zipArchive,
        IReadOnlyList<ModPackage> modPackages, string? orderName = null)
    {
        ZipArchiveEntry? orderEntry = null;

        var orderNameEntry = zipArchive.GetEntry(orderName + ".json");
        var currentEntry = zipArchive.GetEntry("Current.json");

        if (orderName != null && orderNameEntry != null)
        {
            orderEntry = orderNameEntry;
        }
        else if (currentEntry != null)
        {
            orderEntry = currentEntry;
        }
        else
        {
            foreach (var entry in zipArchive.Entries)
            {
                if (entry.FullName.EndsWith(".json"))
                {
                    orderEntry = entry;
                }
            }
        }

        if (orderEntry == null)
        {
            return null;
        }

        await using var orderEntryStream = orderEntry.Open();
        var document = await JsonDocument.ParseAsync(orderEntryStream).ConfigureAwait(false);
        var order = await Task.Run(() => LoadJsonOrder(document.RootElement, modPackages)).ConfigureAwait(false);

        return order;
    }
}
