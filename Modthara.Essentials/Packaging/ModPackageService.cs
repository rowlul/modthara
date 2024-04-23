﻿using System.IO.Abstractions;
using System.Text.RegularExpressions;

using Modthara.Essentials.Abstractions;
using Modthara.Lari;
using Modthara.Lari.Lsx;
using Modthara.Lari.Pak;

using Index = int;

namespace Modthara.Essentials.Packaging;

public delegate void PackageReadCallback(Index idx, ModPackage package);

/// <inheritdoc cref="IModPackageService"/>
public partial class ModPackageService : IModPackageService
{
    private readonly IFileSystem _fileSystem;

    public ModPackageService(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    /// <inheritdoc />
    public async ValueTask<ModPackage> ReadPackageAsync(string path)
    {
        var fileStream = _fileSystem.FileStream.New(path, FileMode.Open, FileAccess.Read,
            FileShare.Read, bufferSize: 4096, useAsync: true);

        try
        {
            var pak = await Task.Run(() => PackageReader.FromStream(fileStream)).ConfigureAwait(false);

            var t1 = Task.Run(() => FindMeta(pak));
            var t2 = Task.Run(() => HasModFiles(pak));
            var t3 = Task.Run(() => AltersGameFiles(pak));
            var t4 = Task.Run(() => HasForceRecompile(pak));
            var t5 = Task.Run(() => RequiresScriptExtender(pak));

            await Task.WhenAll(t1, t2, t3, t4, t5).ConfigureAwait(false);

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
                LastModified = lastModified,
                Path = path
            };
            return modPackage;
        }
        finally
        {
            await fileStream.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<ModPackage> ReadPackagesAsync(string path, Action<Exception>? onException = null,
        PackageReadCallback? packageReadCallback = null)
    {
        int i = 0;
        foreach (var file in _fileSystem.Directory.EnumerateFiles(path, "*.pak", SearchOption.TopDirectoryOnly))
        {
            i++;

            ModPackage modPackage;
            try
            {
                modPackage = await ReadPackageAsync(file).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                onException?.Invoke(ex);
                continue;
            }

            packageReadCallback?.Invoke(i, modPackage);
            yield return modPackage;
        }
    }

    /// <inheritdoc />
    public int CountPackages(string path) =>
        _fileSystem.Directory.GetFiles(path, "*.pak", SearchOption.TopDirectoryOnly).Length;

    public PackagedFile? FindMeta(Package pak)
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

    public PackagedFile? FindScriptExtenderConfig(Package pak)
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

    public ModFlags RequiresScriptExtender(Package pak) =>
        FindScriptExtenderConfig(pak) != null ? ModFlags.RequiresScriptExtender : ModFlags.None;

    public ModFlags HasForceRecompile(Package pak) =>
        pak.Files.Select(file => ForceRecompileRegex().Match(file.Name)).Any(match => match.Success)
            ? ModFlags.HasModFixer
            : ModFlags.None;

    public ModFlags AltersGameFiles(Package pak) =>
        pak.Files.Any(file => file.Name.StartsWith("Video/") ||
                              CommonAlteredGameFiles.Any(f =>
                                  file.Name.StartsWith("Public/" + f) ||
                                  file.Name.StartsWith("Generated/Public/" + f) ||
                                  file.Name.StartsWith($"Mods/{f}/Public") ||
                                  file.Name.StartsWith($"Mods/{f}/Generated/Public")))
            ? ModFlags.AltersGameFiles
            : ModFlags.None;

    public ModFlags HasModFiles(Package pak)
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
    private static readonly string[] CommonAlteredGameFiles =
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