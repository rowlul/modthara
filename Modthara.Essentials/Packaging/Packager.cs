using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Text.RegularExpressions;

using Modthara.Essentials.Abstractions;
using Modthara.Lari;
using Modthara.Lari.Lsx;
using Modthara.Lari.Pak;

using Index = int;

namespace Modthara.Essentials.Packaging;

public delegate Task AsyncPackageCallback(Index idx, ModPackage package);

/// <inheritdoc cref="IPackager"/>
public partial class Packager : IPackager
{
    private readonly string _modsPath;
    private readonly IFileSystem _fileSystem;

    private readonly List<ModPackage> _cache = [];

    /// <inheritdoc cref="IPackager.Cache"/>
    public IEnumerable<ModPackage> Cache => _cache;

    /// <summary>
    /// Creates an instance of the service.
    /// </summary>
    /// <param name="modsPath">Path to the user Mods folder.</param>
    /// <param name="fileSystem">File system abstraction.</param>
    public Packager(string modsPath, IFileSystem fileSystem)
    {
        _modsPath = modsPath;
        _fileSystem = fileSystem;
    }

    /// <inheritdoc cref="IPackager.ReadPackages"/>
    public IEnumerable<ModPackage> ReadPackages()
    {
        List<ModPackage> modPackages = [];

        foreach (var path in _fileSystem.Directory.EnumerateFiles(_modsPath, "*.pak", SearchOption.TopDirectoryOnly))
        {
            using var fileStream = _fileSystem.FileStream.New(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var pak = PackageReader.FromStream(fileStream);
            var modPackage = CreateModPackage(pak, path);
            modPackages.Add(modPackage);
        }

        return modPackages;
    }

    /// <inheritdoc cref="IPackager.ReadPackages"/>
    public async IAsyncEnumerable<ModPackage> ReadPackagesAsync()
    {
        using var e = await Task.Run([SuppressMessage("ReSharper", "NotDisposedResourceIsReturned")]() =>
                _fileSystem.Directory.EnumerateFiles(_modsPath, "*.pak", SearchOption.TopDirectoryOnly).GetEnumerator())
            .ConfigureAwait(false);

        while (await Task.Run(() => e.MoveNext()).ConfigureAwait(false))
        {
            var path = e.Current;
            var fileStream = _fileSystem.FileStream.New(path, FileMode.Open, FileAccess.Read,
                FileShare.Read, bufferSize: 4096, useAsync: true);
            var pak = await Task.Run(() => PackageReader.FromStream(fileStream)).ConfigureAwait(false);
            var modPackage = await CreateModPackageAsync(pak, path).ConfigureAwait(false);
            await fileStream.DisposeAsync().ConfigureAwait(false);
            yield return modPackage;
        }
    }

    /// <inheritdoc cref="IPackager.LoadPackagesToCacheAsync"/>
    public async Task LoadPackagesToCacheAsync(AsyncPackageCallback? asyncPackageCallback = null)
    {
        await using var e = ReadPackagesAsync().GetAsyncEnumerator();

        int i = 0;
        while (await e.MoveNextAsync().ConfigureAwait(false))
        {
            i++;
            if (asyncPackageCallback != null)
            {
                await asyncPackageCallback.Invoke(i, e.Current).ConfigureAwait(false);
            }
            _cache.Add(e.Current);
        }
    }

    /// <inheritdoc cref="IPackager.Count"/>
    public int Count() =>
        _fileSystem.Directory.EnumerateFiles(_modsPath, "*.pak", SearchOption.TopDirectoryOnly).Count();

    public async ValueTask<int> CountAsync()
    {
        using var e = await Task.Run([SuppressMessage("ReSharper", "NotDisposedResourceIsReturned")]() =>
                _fileSystem.Directory.EnumerateFiles(_modsPath, "*.pak", SearchOption.TopDirectoryOnly).GetEnumerator())
            .ConfigureAwait(false);

        int i = 0;
        while (await Task.Run(() => e.MoveNext()).ConfigureAwait(false))
        {
            i++;
        }

        return i;
    }

    internal ModPackage CreateModPackage(Package pak, string path)
    {
        var meta = FindMeta(pak);

        var hasModFiles = HasModFiles(pak);
        var altersGameFiles = AltersGameFiles(pak);
        var hasForceRecompile = HasForceRecompile(pak);
        var requiresScriptExtender = RequiresScriptExtender(pak);
        var flags = hasModFiles | altersGameFiles | hasForceRecompile | requiresScriptExtender;

        var lastModified = _fileSystem.FileInfo.New(path).LastWriteTime;

        ModMetadata modMeta;
        if (meta != null)
        {
            using var metaStream = meta.Open();
            var lsx = LsxDocument.FromStream(metaStream);
            modMeta = ModMetadata.FromLsx(lsx);
        }
        else
        {
            modMeta = new ModMetadata
            {
                Name = _fileSystem.Path.GetFileNameWithoutExtension(path), FolderName = string.Empty
            };
        }

        var modPackage = new ModPackage
        {
            Name = modMeta.Name,
            Author = modMeta.Author,
            Description = modMeta.Description,
            FolderName = modMeta.FolderName,
            Md5 = modMeta.Md5,
            Uuid = modMeta.Uuid,
            Version = modMeta.Version,
            Dependencies = modMeta.Dependencies,
            Package = pak,
            Flags = flags,
            LastModified = lastModified
        };
        return modPackage;
    }

    internal async ValueTask<ModPackage> CreateModPackageAsync(Package pak, string path)
    {
        var t1 = Task.Run(() => FindMeta(pak));
        var t2 = Task.Run(() => HasModFiles(pak));
        var t3 = Task.Run(() => AltersGameFiles(pak));
        var t4 = Task.Run(() => HasForceRecompile(pak));
        var t5 = Task.Run(() => RequiresScriptExtender(pak));

        List<Task> tasks = [t1, t2, t3, t4, t5];
        await Task.WhenAll(tasks).ConfigureAwait(false);

        var meta = t1.Result;
        var hasModFiles = t2.Result;
        var altersGameFiles = t3.Result;
        var hasForceRecompile = t4.Result;
        var requiresScriptExtender = t5.Result;

        var flags = hasModFiles | altersGameFiles | hasForceRecompile | requiresScriptExtender;

        var lastModified = _fileSystem.FileInfo.New(path).LastWriteTime;

        ModMetadata modMeta;
        if (meta != null)
        {
            var metaStream = meta.Open();
            var lsx = await Task.Run(() => LsxDocument.FromStream(metaStream)).ConfigureAwait(false);
            modMeta = await Task.Run(() => ModMetadata.FromLsx(lsx)).ConfigureAwait(false);
            await metaStream.DisposeAsync().ConfigureAwait(false);
        }
        else
        {
            modMeta = new ModMetadata
            {
                Name = _fileSystem.Path.GetFileNameWithoutExtension(path), FolderName = string.Empty
            };
        }

        var modPackage = new ModPackage
        {
            Name = modMeta.Name,
            Author = modMeta.Author,
            Description = modMeta.Description,
            FolderName = modMeta.FolderName,
            Md5 = modMeta.Md5,
            Uuid = modMeta.Uuid,
            Version = modMeta.Version,
            Dependencies = modMeta.Dependencies,
            Package = pak,
            Flags = flags,
            LastModified = lastModified
        };
        return modPackage;
    }

    internal PackagedFile? FindMeta(Package pak)
    {
        foreach (var file in pak.Files)
        {
            var match = MetaRegex().Match(file.Name);
            if (match.Success)
            {
                return file;
            }
        }

        return null;
    }

    internal PackagedFile? FindScriptExtenderConfig(Package pak)
    {
        foreach (var file in pak.Files)
        {
            var match = ScriptExtenderConfigRegex().Match(file.Name);
            if (match.Success)
            {
                return file;
            }
        }

        return null;
    }

    internal ModFlags RequiresScriptExtender(Package pak) =>
        FindScriptExtenderConfig(pak) != null ? ModFlags.RequiresScriptExtender : ModFlags.None;

    internal ModFlags HasForceRecompile(Package pak) =>
        pak.Files.Select(file => ForceRecompileRegex().Match(file.Name)).Any(match => match.Success)
            ? ModFlags.HasModFixer
            : ModFlags.None;

    internal ModFlags AltersGameFiles(Package pak) =>
        pak.Files.Any(file => file.Name.StartsWith("Video/") ||
                              CommonAlteredGameFiles.Any(f =>
                                  file.Name.StartsWith("Public/" + f) ||
                                  file.Name.StartsWith("Generated/Public/" + f) ||
                                  file.Name.StartsWith($"Mods/{f}/Public") ||
                                  file.Name.StartsWith($"Mods/{f}/Generated/Public")))
            ? ModFlags.AltersGameFiles
            : ModFlags.None;

    internal ModFlags HasModFiles(Package pak)
    {
        foreach (var file in pak.Files)
        {
            var scriptsScript = file.Name.StartsWith("Scripts/");
            if (scriptsScript)
            {
                return ModFlags.HasModFiles;
            }

            var localizationMatch = LocalizationRegex().Match(file.Name);
            if (localizationMatch.Success &&
                (!file.Name.EndsWith($"{localizationMatch.Groups[1].Value.ToLower()}.loca.xml") ||
                 !file.Name.EndsWith($"{localizationMatch.Groups[1].Value.ToLower()}.xml")))
            {
                return ModFlags.HasModFiles;
            }

            var modsFolderMatch = ModsFolderRegex().Match(file.Name);
            if (modsFolderMatch.Success && CommonAlteredGameFiles.All(f => f != modsFolderMatch.Groups[1].Value) &&
                file.Name != $"Mods/{modsFolderMatch.Groups[1].Value}/meta.lsx" &&
                file.Name != $"Mods/{modsFolderMatch.Groups[1].Value}/Story/RawFiles/Goals/ForceRecompile.txt")
            {
                return ModFlags.HasModFiles;
            }

            var publicFolderMatch = PublicFolderRegex().Match(file.Name);
            if (publicFolderMatch.Success && CommonAlteredGameFiles.All(f => f != publicFolderMatch.Groups[1].Value))
            {
                return ModFlags.HasModFiles;
            }
        }

        return ModFlags.None;
    }

    // TODO: this could be a dynamic list of files in user's Data/ folder
    private static readonly ImmutableHashSet<string> CommonAlteredGameFiles =
    [
        "Assets",
        "Game",
        "Engine",
        "Gustav",
        "GustavDev",
        "Shared",
        "SharedDev"
    ];

    [GeneratedRegex("^Mods/([^/]+)/Story/RawFiles/Goals/ForceRecompile\\.txt$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex ForceRecompileRegex();

    [GeneratedRegex("^Mods/([^/]+)/meta\\.lsx$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex MetaRegex();

    [GeneratedRegex("^Localization/([^/]+)/([^/]+)(?:\\.loca)?\\.xml$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex LocalizationRegex();

    [GeneratedRegex("^Mods/([^/]+)/.*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex ModsFolderRegex();

    [GeneratedRegex("^Public/([^/]+)/.*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex PublicFolderRegex();

    [GeneratedRegex("^Mods/([^/]+)/ScriptExtender/Config\\.json$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex ScriptExtenderConfigRegex();
}
