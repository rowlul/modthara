using System.IO.Abstractions;
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

        List<ModMetadata> mods = [];
        foreach (var uuid in document.RootElement.GetProperty("Order").EnumerateArray()
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
        var order = new JsonArray();

        foreach (var modMeta in modSettings.Mods)
        {
            var mod = new JsonObject { { "UUID", modMeta.Uuid.Value }, { "Name", modMeta.Name } };
            order.Add(mod);
        }

        var root = new JsonObject { { "Order", order } };
        await _fileSystem.File.WriteAllTextAsync(path,
                root.ToJsonString(new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    AllowTrailingCommas = false
                }))
            .ConfigureAwait(false);
    }
}
