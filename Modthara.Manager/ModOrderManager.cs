using System.IO.Abstractions;
using System.Text.Json;

using static Modthara.Manager.Constants;

namespace Modthara.Manager;

/// <inheritdoc />
public class ModOrderManager : IModOrderManager
{
    private readonly IFileSystem _fileSystem;

    public ModOrderManager(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public async ValueTask<ModOrder?> LoadOrderAsync(string path)
    {
        await using var file = _fileSystem.FileStream.New(path, FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: StreamBufferSize, useAsync: true);

        var order = JsonSerializer.DeserializeAsync<ModOrder>(file, Constants.JsonSerializerOptions)
            .ConfigureAwait(false);
        return await order;
    }

    public async Task SaveOrderAsync(string path, ModOrder order)
    {
        await using var file = _fileSystem.FileStream.New(path, FileMode.Create, FileAccess.Write, FileShare.Read,
            bufferSize: StreamBufferSize, useAsync: true);

        await JsonSerializer.SerializeAsync(file, order, Constants.JsonSerializerOptions).ConfigureAwait(false);
    }
}
