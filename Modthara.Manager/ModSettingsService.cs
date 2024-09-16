using System.IO.Abstractions;

using Modthara.Lari;
using Modthara.Lari.Lsx;

using static Modthara.Manager.Constants;

namespace Modthara.Manager;

public class ModSettingsService
{
    private readonly IFileSystem _fileSystem;

    public ModSettingsService(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public ValueTask<ModSettings> ReadModSettingsAsync(string path)
    {
        var stream = _fileSystem.FileStream.New(path, FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: StreamBufferSize, useAsync: true);
        return ReadModSettingsAsync(stream);
    }

    public async ValueTask<ModSettings> ReadModSettingsAsync(Stream stream)
    {
        var lsx = await Task.Run(() => LsxDocument.FromStream(stream)).ConfigureAwait(false);
        await stream.DisposeAsync().ConfigureAwait(false);

        var modSettings = new ModSettings(lsx);
        modSettings.Sanitize();
        return modSettings;
    }

    public async Task WriteModSettingsAsync(ModSettings modSettings, string path)
    {
        await using var stream = modSettings.ToDocument().ToStream();
        await using var file = _fileSystem.FileStream.New(path, FileMode.Create, FileAccess.Write, FileShare.Read,
            bufferSize: StreamBufferSize, useAsync: true);
        await stream.CopyToAsync(file).ConfigureAwait(false);
    }
}
