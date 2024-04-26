using System.IO.Abstractions;

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
    public async Task<ModSettings> ReadModSettingsAsync(string path)
    {
        await using var file = _fileSystem.FileStream.New(path, FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: 4096, useAsync: true);

        var lsx = LsxDocument.FromStream(file);
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
}
