
using System.IO.Abstractions;
using System.Text.Json;

using static Modthara.Manager.Constants;

namespace Modthara.Manager;

public class ModManagerSettingsService : IModManagerSettingsService
{
    private readonly string _path;

    private readonly IFileSystem _fileSystem;

    private ModManagerSettings? _settings;
    
    public ModManagerSettingsService(string path, IFileSystem fs)
    {
        _fileSystem = fs;
        _path = path;
    }

    public async Task LoadSettingsAsync()
    {
        var file = _fileSystem.FileStream.New(_path, FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: StreamBufferSize, useAsync: true);

        var settings = await JsonSerializer.DeserializeAsync<ModManagerSettings>(file, Constants.JsonSerializerOptions).ConfigureAwait(false);
        await Task.Run(() => {
            _settings = settings!;
            }
        );
    }

    public async Task SaveSettingsAsync()
    {
        await using var file = _fileSystem.FileStream.New(_path, FileMode.Create, FileAccess.Write, FileShare.Read,
            bufferSize: StreamBufferSize, useAsync: true);

        await JsonSerializer.SerializeAsync(file, _settings, Constants.JsonSerializerOptions).ConfigureAwait(false);
    }

    public string GetBinDirectoryPath()
    {
        return _settings!._binPath;
    }

    public string GetDataDirectoryPath()
    {
        return _settings!._dataPath;
    }

    public string GetModsDirectoryPath()
    {
        return _settings!._modsPath;
    }

    public string GetModSettingsPath()
    {
        return _settings!._modsettingsPath;
    }
}