using System.IO.Abstractions;
using System.IO.Compression;

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
    public async Task LoadModPackagesAsync(Action<int, Exception>? onException = null,
        Action<int, ModPackage>? onPackageRead = null)
    {
        await foreach (var package in _modPackageManager.ReadModPackagesAsync(_path, onException, onPackageRead)
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
    public async Task ImportModPackageAsync(ModPackage modPackage)
    {
        var destPath = _fileSystem.Path.Combine(_path, modPackage.Path);

        await using var sourceStream = _fileSystem.FileStream.New(modPackage.Path, FileMode.Open, FileAccess.Read,
            FileShare.Read, StreamBufferSize, useAsync: true);

        await using var destStream = _fileSystem.FileStream.New(destPath, FileMode.Create, FileAccess.Write,
            FileShare.None, StreamBufferSize, useAsync: true);

        await sourceStream.CopyToAsync(destStream, StreamBufferSize).ConfigureAwait(false);

        _modPackages.Add(modPackage);
    }

    /// <inheritdoc />
    public async Task ImportArchivedModPackagesAsync(ZipArchive zipArchive, Action<Exception>? onException = null,
        PackageReadCallback? packageReadCallback = null)
    {
        int i;
        for (i = 0; i < zipArchive.Entries.Count; i++)
        {
            var entry = zipArchive.Entries[i];
            if (entry.FullName.EndsWith(".pak", StringComparison.OrdinalIgnoreCase))
            {
                await using var pakStream = entry.Open();

                var modPackagePath = _fileSystem.Path.Combine(_path, entry.FullName);

                try
                {
                    var modPackage = await _modPackageManager.ReadModPackageAsync(pakStream, modPackagePath, true)
                        .ConfigureAwait(false);
                    packageReadCallback?.Invoke(i, modPackage);

                    _modPackages.Add(modPackage);

                    pakStream.Position = 0;

                    await using var pakFileStream = _fileSystem.FileStream.New(modPackagePath, FileMode.Create,
                        FileAccess.Write,
                        FileShare.None, 4096, useAsync: true);
                    await pakStream.CopyToAsync(pakFileStream).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    if (onException != null)
                    {
                        onException(e);
                    }

                    throw;
                }
            }
        }
    }

    /// <inheritdoc />
    public void EnableModPackage(ModPackage modPackage)
    {
        var newPath = _fileSystem.Path.ChangeExtension(modPackage.Path, null);
        _fileSystem.FileInfo.New(modPackage.Path).MoveTo(newPath);

        modPackage.Path = newPath;
        modPackage.Flags |= ModFlags.Enabled;
    }

    /// <inheritdoc />
    public void DisableModPackage(ModPackage modPackage)
    {
        var newPath = _fileSystem.Path.ChangeExtension(modPackage.Path, ".pak.off");
        _fileSystem.FileInfo.New(modPackage.Path).MoveTo(newPath);

        modPackage.Path = newPath;
        modPackage.Flags &= ~ModFlags.Enabled;
    }

    /// <inheritdoc />
    public void DeleteModPackage(ModPackage modPackage)
    {
        _fileSystem.Directory.Delete(modPackage.Path);
        _modPackages.Remove(modPackage);
    }

    private const int StreamBufferSize = 0x1000;
}
