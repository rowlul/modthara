using System.IO.Abstractions;
using System.Text.Json;

using Humanizer;

using Modthara.Manager;

namespace Modthara.UI.Services;

public class ConfigurationService : IPathProvider
{
    private readonly IFileSystem _fileSystem;
    private readonly string _path;

    private readonly JsonSerializerOptions _jsonSerializerOptions =
        new() { WriteIndented = true, IgnoreReadOnlyProperties = true };

    public ConfigurationModel Instance { get; private set; }

    public string LocalLarianFolder => Instance.LocalLarianFolder;

    public ConfigurationService(IFileSystem fileSystem, string path, ConfigurationModel defaultConfig)
    {
        _fileSystem = fileSystem;
        _path = path;

        Instance = defaultConfig;
    }

    public void LoadConfiguration()
    {
        if (!_fileSystem.File.Exists(_path))
        {
            return;
        }

        using var stream = _fileSystem.FileStream.New(_path, FileMode.Open, FileAccess.Read, FileShare.Read);
        var model = JsonSerializer.Deserialize<ConfigurationModel>(stream, _jsonSerializerOptions);
        if (model != null)
        {
            Instance = model;
        }
    }

    public async Task LoadConfigurationAsync()
    {
        if (!_fileSystem.File.Exists(_path))
        {
            return;
        }

        await using var stream = _fileSystem.FileStream.New(_path, FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: 4096, useAsync: true);
        var model = await JsonSerializer.DeserializeAsync<ConfigurationModel>(stream, _jsonSerializerOptions);
        if (model != null)
        {
            Instance = model;
        }
    }

    public void SaveConfiguration()
    {
        using var stream = _fileSystem.FileStream.New(_path, FileMode.Create, FileAccess.Write, FileShare.Read);
        JsonSerializer.Serialize(stream, Instance, _jsonSerializerOptions);
    }

    public async Task SaveConfigurationAsync()
    {
        await using var stream = _fileSystem.FileStream.New(_path, FileMode.Create, FileAccess.Write, FileShare.Read,
            bufferSize: 4096, useAsync: true);
        await JsonSerializer.SerializeAsync(stream, Instance, _jsonSerializerOptions);
    }
}
