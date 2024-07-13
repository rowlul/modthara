using System.IO.Abstractions;
using System.Text.Json;

using static Modthara.Manager.Constants;

namespace Modthara.Manager;

public class ModOrderManager : IModOrderManager
{
    private readonly IFileSystem _fileSystem;

    public ModOrderManager(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<ModOrderEntry> LoadOrderAsync(string path)
    {
        await using var file = _fileSystem.FileStream.New(path, FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: StreamBufferSize, useAsync: true);

        var entries = JsonSerializer.DeserializeAsyncEnumerable<ModOrderEntry>(file, Constants.JsonSerializerOptions);
        await foreach (var entry in entries.ConfigureAwait(false))
        {
            if (entry == null)
            {
                continue;
            }

            yield return entry;
        }
    }

    /// <inheritdoc />
    public async Task SaveOrderAsync(string path, IEnumerable<ModOrderEntry> orderEntries)
    {
        await using var file = _fileSystem.FileStream.New(path, FileMode.Create, FileAccess.Write, FileShare.Read,
            bufferSize: StreamBufferSize, useAsync: true);

        await JsonSerializer.SerializeAsync(file, orderEntries, Constants.JsonSerializerOptions).ConfigureAwait(false);
    }
}
