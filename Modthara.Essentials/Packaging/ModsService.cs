using System.IO.Abstractions;

namespace Modthara.Essentials.Packaging;

/// <inheritdoc />
public class ModsService : IModsService
{
    private readonly string _path;

    private readonly IModPackageManager _modPackageManager;
    private readonly IFileSystem _fileSystem;

    private readonly List<ModPackage> _modPackages = [];

    /// <inheritdoc />
    public IReadOnlyList<ModPackage> ModPackages => _modPackages;

    public ModsService(string path, IModPackageManager modPackageManager, IFileSystem fileSystem)
    {
        _path = path;

        _modPackageManager = modPackageManager;
        _fileSystem = fileSystem;
    }

    /// <inheritdoc />
    public async Task LoadModPackagesAsync(Action<Exception>? onException = null,
        PackageReadCallback? packageReadCallback = null)
    {
        await foreach (var package in _modPackageManager.ReadModPackagesAsync(_path, onException, packageReadCallback)
                           .ConfigureAwait(false))
        {
            _modPackages.Add(package);
        }
    }

    /// <inheritdoc />
    public int CountModPackages() => _modPackageManager.CountModPackages(_path);

    /// <inheritdoc />
    public IReadOnlySet<(ModPackage, ModPackage)> FindDuplicateModPackages()
    {
        HashSet<(ModPackage, ModPackage)> duplicates = [];

        foreach (var p1 in _modPackages)
        {
            foreach (var p2 in _modPackages.Where(p2 => p1.Uuid == p2.Uuid || p1.FolderName == p2.FolderName))
            {
                duplicates.Add((p1, p2));
            }
        }

        return duplicates;
    }

    /// <inheritdoc />
    public async ValueTask<ModPackage> ImportModPackageAsync(string path)
    {
        var package = await _modPackageManager.ReadModPackageAsync(path).ConfigureAwait(false);

        var filename = _fileSystem.Path.GetFileName(path);
        var destPath = _fileSystem.Path.Combine(path, filename);

        const int bufferSize = 4096;

        await using var sourceStream = _fileSystem.FileStream.New(path, FileMode.Open, FileAccess.Read,
            FileShare.Read, bufferSize, useAsync: true);

        await using var destStream = _fileSystem.FileStream.New(destPath, FileMode.Open, FileAccess.Read,
            FileShare.Read, bufferSize, useAsync: true);

        await sourceStream.CopyToAsync(destStream, bufferSize).ConfigureAwait(false);

        _modPackages.Add(package);
        return package;
    }

    /// <inheritdoc />
    public void DeleteModPackage(string path)
    {
        var package = _modPackages.FirstOrDefault(x => x.Path == path);
        if (package == null)
        {
            return;
        }

        _fileSystem.Directory.Delete(path);
        _modPackages.Remove(package);
    }
}
