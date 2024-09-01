using System.IO.Abstractions;

using Modthara.Lari;
using Modthara.Lari.Lsx;

using static Modthara.Manager.Constants;

namespace Modthara.Manager;

/// <inheritdoc />
public class ModSettingsService : IModSettingsService
{
    private string _path;
    public string Path { 
        get => _path;
        set {
            _path = value;
            ModSettings = null;
        }
    }

    private readonly IFileSystem _fileSystem;

    public ModSettings? ModSettings { get; private set; }

    public ModSettingsService(string path, IFileSystem fileSystem)
    {
        _path = path;

        _fileSystem = fileSystem;
    }

    public async Task LoadModSettingsAsync()
    {
        var file = _fileSystem.FileStream.New(_path, FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: StreamBufferSize, useAsync: true);

        var lsx = await Task.Run(() => LsxDocument.FromStream(file)).ConfigureAwait(false);
        await file.DisposeAsync().ConfigureAwait(false);

        var modSettings = new ModSettings(lsx);
        await Task.Run(() => modSettings.Sanitize()).ConfigureAwait(false);

        ModSettings = modSettings;
    }

    public async Task SaveModSettingsAsync()
    {
        await using var stream = ModSettings.ToDocument().ToStream();
        await using var file = _fileSystem.FileStream.New(_path, FileMode.Create, FileAccess.Write, FileShare.None,
            bufferSize: StreamBufferSize, useAsync: true);
        await stream.CopyToAsync(file).ConfigureAwait(false);
    }
}
