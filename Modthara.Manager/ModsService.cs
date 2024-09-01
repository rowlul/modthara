using System.IO.Abstractions;
using System.IO.Compression;

using Modthara.Lari;
using Modthara.Manager.Extensions;

using static Modthara.Manager.Constants;

namespace Modthara.Manager;

/// <inheritdoc />
public class ModsService : IModsService
{
    private string _path;
    public string Path { 
        get => _path;
        set {
            _path = value;
            _modPackages = [];
        }
    }

    private readonly IFileSystem _fileSystem;
    private readonly IModPackageManager _modPackageManager;

    private List<ModPackage> _modPackages = [];

    public IReadOnlyList<ModPackage> ModPackages => _modPackages;

    public ModsService(string path, IFileSystem fileSystem, IModPackageManager modPackageManager)
    {
        _path = path;

        _fileSystem = fileSystem;
        _modPackageManager = modPackageManager;
    }

    public async Task LoadModPackagesAsync(Action<int, Exception>? onException = null,
        Action<int, ModPackage>? onPackageRead = null)
    {
        if (_modPackages.Count > 0)
        {
            _modPackages.Clear();
        }

        await foreach (var package in _modPackageManager.ReadModPackagesAsync(_path, onException, onPackageRead)
                           .ConfigureAwait(false))
        {
            _modPackages.Add(package);
        }
    }

    public async ValueTask<(IEnumerable<ModPackage> foundMods, IEnumerable<Module> missingMods)>
        GetModsFromModSettingsAsync(
            ModSettings modSettings)
    {
        List<ModPackage> foundMods = [];
        List<Module> missingMods = [];

        foreach (var mod in modSettings.Mods)
        {
            var t1 = Task.Run(() => mod.IsGameFile());
            var t2 = Task.Run(() => _modPackages.FindMatchingMod(mod));
            await Task.WhenAll(t1, t2).ConfigureAwait(false);

            var isGameFile = t1.Result;
            if (isGameFile)
            {
                continue;
            }

            var modPackage = t2.Result;
            if (modPackage != null)
            {
                foundMods.Add(modPackage);
            }
            else
            {
                missingMods.Add(mod);
            }
        }

        return (foundMods, missingMods);
    }

    public async ValueTask<(IEnumerable<ModPackage> foundMods, IEnumerable<ModOrderEntry> missingMods)>
        GetModsFromOrderAsync(
            IAsyncEnumerable<ModOrderEntry> orderEntries)
    {
        List<ModPackage> foundMods = [];
        List<ModOrderEntry> missingMods = [];

        await foreach (var entry in orderEntries.ConfigureAwait(false))
        {
            var t1 = Task.Run(() => entry.IsGameFile());
            var t2 = Task.Run(() => _modPackages.FindMatchingMod(entry));
            await Task.WhenAll(t1, t2).ConfigureAwait(false);

            var isGameFile = t1.Result;
            if (isGameFile)
            {
                continue;
            }

            var modPackage = t2.Result;
            if (modPackage != null)
            {
                foundMods.Add(modPackage);
            }
            else
            {
                missingMods.Add(entry);
            }
        }

        return (foundMods, missingMods);
    }

    public IEnumerable<(ModPackage, ModPackage)> FindDuplicateModPackages()
    {
        var duplicates = new List<(ModPackage, ModPackage)>();

        var groups = _modPackages
            .Where(x => x.Metadata != null)
            .GroupBy(x => x.Metadata!.Uuid);

        foreach (var group in groups)
        {
            if (group.Count() > 1)
            {
                var modPackages = group.ToList();
                for (int i = 0; i < modPackages.Count; i++)
                {
                    for (int j = i + 1; j < modPackages.Count; j++)
                    {
                        duplicates.Add((modPackages[i], modPackages[j]));
                    }
                }
            }
        }

        return duplicates;
    }

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

    public async Task ImportArchivedModPackagesAsync(ZipArchive zipArchive, Action<Exception>? onException = null,
        Action<int, ModPackage>? onPackageRead = null)
    {
        int i;
        for (i = 0; i < zipArchive.Entries.Count; i++)
        {
            var entry = zipArchive.Entries[i];
            if (entry.FullName.EndsWith(".pak", StringComparison.OrdinalIgnoreCase))
            {
                await using var pakStream = entry.Open();
                var modPackagePath = _fileSystem.Path.Combine(_path, entry.FullName);

                ModPackage modPackage;
                try
                {
                    modPackage = await _modPackageManager.ReadModPackageAsync(pakStream, modPackagePath, true)
                        .ConfigureAwait(false);

                    _modPackages.Add(modPackage);

                    pakStream.Position = 0;

                    await using var pakFileStream = _fileSystem.FileStream.New(modPackagePath, FileMode.Create,
                        FileAccess.Write,
                        FileShare.None, StreamBufferSize, useAsync: true);
                    await pakStream.CopyToAsync(pakFileStream).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    onException?.Invoke(e);
                    throw;
                }

                onPackageRead?.Invoke(i, modPackage);
            }
        }
    }

    public void EnableModPackage(ModPackage modPackage)
    {
        var newPath = _fileSystem.Path.ChangeExtension(modPackage.Path, null);
        _fileSystem.FileInfo.New(modPackage.Path).MoveTo(newPath);

        modPackage.Path = newPath;
        modPackage.Flags |= ModFlags.Enabled;
    }

    public void DisableModPackage(ModPackage modPackage)
    {
        var newPath = _fileSystem.Path.ChangeExtension(modPackage.Path, ".pak.off");
        _fileSystem.FileInfo.New(modPackage.Path).MoveTo(newPath);

        modPackage.Path = newPath;
        modPackage.Flags &= ~ModFlags.Enabled;
    }

    public void DeleteModPackage(ModPackage modPackage)
    {
        _fileSystem.File.Delete(modPackage.Path);
        _modPackages.Remove(modPackage);
    }
}
