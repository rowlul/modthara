using System.IO.Abstractions;

using Modthara.Essentials.Abstractions;

namespace Modthara.Essentials.Packaging;

public class UserModsDirectory : IModsDirectory
{
    private readonly string _path;

    private readonly IModPackageService _modPackageService;
    private readonly IFileSystem _fileSystem;

    private readonly List<ModPackage> _packages = [];

    /// <inheritdoc />
    public IReadOnlyList<ModPackage> Packages => _packages;

    public UserModsDirectory(string path, IModPackageService modPackageService, IFileSystem fileSystem)
    {
        _path = path;

        _modPackageService = modPackageService;
        _fileSystem = fileSystem;
    }

    /// <inheritdoc />
    public async Task LoadPackagesAsync(Action<Exception>? onException = null,
        PackageReadCallback? packageReadCallback = null)
    {
        await foreach (var package in _modPackageService.ReadPackagesAsync(_path, onException, packageReadCallback)
                           .ConfigureAwait(false))
        {
            _packages.Add(package);
        }
    }

    /// <inheritdoc />
    public int CountPackages() => _modPackageService.CountPackages(_path);

    /// <inheritdoc />
    public IReadOnlySet<(ModPackage, ModPackage)> FindDuplicates()
    {
        HashSet<(ModPackage, ModPackage)> duplicates = [];

        foreach (var p1 in _packages)
        {
            foreach (var p2 in _packages.Where(p2 => p1.Uuid == p2.Uuid || p1.FolderName == p2.FolderName))
            {
                duplicates.Add((p1, p2));
            }
        }

        return duplicates;
    }

    /// <inheritdoc />
    public async Task<ModPackage> ImportPackageAsync(string path)
    {
        var package = await _modPackageService.ReadPackageAsync(path).ConfigureAwait(false);

        var filename = _fileSystem.Path.GetFileName(path);
        var destPath = _fileSystem.Path.Combine(path, filename);

        const int bufferSize = 4096;

        await using var sourceStream = _fileSystem.FileStream.New(path, FileMode.Open, FileAccess.Read,
            FileShare.Read, bufferSize, useAsync: true);

        await using var destStream = _fileSystem.FileStream.New(destPath, FileMode.Open, FileAccess.Read,
            FileShare.Read, bufferSize, useAsync: true);

        await sourceStream.CopyToAsync(destStream, bufferSize).ConfigureAwait(false);

        _packages.Add(package);
        return package;
    }

    /// <inheritdoc />
    public void DeletePackage(string path)
    {
        var package = _packages.FirstOrDefault(x => x.Path == path);
        if (package == null)
        {
            return;
        }

        _fileSystem.Directory.Delete(path);
        _packages.Remove(package);
    }
}
