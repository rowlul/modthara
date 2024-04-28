using System.IO.Abstractions;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Nodes;

using Modthara.Lari;
using Modthara.Lari.Lsx;

namespace Modthara.Essentials.Packaging;

/// <inheritdoc />
public class ModSettingsManager : IModSettingsManager
{
    private readonly IFileSystem _fileSystem;

    public ModSettingsManager(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    /// <inheritdoc />
    public async ValueTask<ModSettings> LoadModSettingsAsync(string path)
    {
        var file = _fileSystem.FileStream.New(path, FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: 4096, useAsync: true);
        var lsx = await Task.Run(() => LsxDocument.FromStream(file)).ConfigureAwait(false);
        await file.DisposeAsync().ConfigureAwait(false);

        var order = new ModSettings(lsx);
        await Task.Run(() => order.Sanitize()).ConfigureAwait(false);

        return order;
    }

    /// <inheritdoc />
    public async Task SaveModSettingsAsync(string path, ModSettings modSettings)
    {
        await using var stream = modSettings.ToStream();
        await using var file = _fileSystem.FileStream.New(path, FileMode.Create, FileAccess.Write, FileShare.None,
            bufferSize: 4096, useAsync: true);
        await stream.CopyToAsync(file).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<ModSettings> LoadJsonOrderAsync(string path, IReadOnlyList<ModPackage> modPackages)
    {
        await using var file = _fileSystem.FileStream.New(path, FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: 4096, useAsync: true);

        var document = await JsonDocument.ParseAsync(file).ConfigureAwait(false);
        var order = await Task.Run(() => LoadJsonOrder(document.RootElement, modPackages)).ConfigureAwait(false);
        return order;
    }

    /// <inheritdoc />
    public ModSettings LoadJsonOrder(JsonElement rootElement, IReadOnlyList<ModPackage> modPackages)
    {
        List<ModMetadata> mods = [];
        foreach (var uuid in rootElement.GetProperty("Order").EnumerateArray()
                     .Select(el => el.GetProperty("UUID").GetString()).OfType<string>())
        {
            ModPackage mod;

            try
            {
                mod = modPackages.First(x => x.Uuid.Value == uuid);
            }
            catch (InvalidOperationException e)
            {
                throw new ModNotFoundException($"Parsed mod not found: {uuid}.", new LariUuid(uuid), e);
            }

            mods.Add(mod);
        }

        var modSettings = new ModSettings(mods: mods);
        return modSettings;
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
    public async ValueTask<ModSettings?> ExtractJsonOrderAsync(ZipArchive zipArchive,
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
