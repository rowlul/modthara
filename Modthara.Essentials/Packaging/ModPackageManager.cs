using System.IO.Abstractions;
using System.Text.RegularExpressions;

using Modthara.Lari;
using Modthara.Lari.Lsx;
using Modthara.Lari.Pak;

namespace Modthara.Essentials.Packaging;

public delegate void PackageReadCallback(Index idx, ModPackage package);

/// <inheritdoc />
public partial class ModPackageManager : IModPackageManager
{
    private readonly IFileSystem _fileSystem;

    public ModPackageManager(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public ValueTask<ModPackage> ReadModPackageAsync(string path, bool leaveOpen = false)
    {
        var fileStream = _fileSystem.FileStream.New(path, FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: 4096, useAsync: true);

        return ReadModPackageAsync(fileStream, path, leaveOpen);
    }

    public async ValueTask<ModPackage> ReadModPackageAsync(Stream stream, string path, bool leaveOpen = false)
    {
        try
        {
            var pak = await Task.Run(() => PackageReader.FromStream(stream)).ConfigureAwait(false);

            var t1 = Task.Run(() => FindMeta(pak));
            var t2 = Task.Run(() => HasModFiles(pak));
            var t3 = Task.Run(() => AltersGameFiles(pak));
            var t4 = Task.Run(() => HasForceRecompile(pak));
            var t5 = Task.Run(() => FindScriptExtenderConfig(pak));

            await Task.WhenAll(t1, t2, t3, t4, t5).ConfigureAwait(false);

            var meta = t1.Result;
            var hasModFiles = t2.Result;
            var altersGameFiles = t3.Result;
            var hasForceRecompile = t4.Result;
            var requiresScriptExtender = t5.Result != null ? ModFlags.RequiresScriptExtender : ModFlags.None;

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
                Path = path,
                ScriptExtenderConfig = t5.Result
            };
            return modPackage;
        }
        finally
        {
            if (!leaveOpen)
            {
                await stream.DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    public async IAsyncEnumerable<ModPackage> ReadModPackagesAsync(string path,
        Action<int, Exception>? onException = null,
        Action<int, ModPackage>? onPackageRead = null)
    {
        var patterns = new[] { "*.pak", "*.pak.off" };

        int i = 0;
        foreach (var pattern in patterns)
        {
            foreach (var file in _fileSystem.Directory.EnumerateFiles(path, pattern, SearchOption.TopDirectoryOnly))
            {
                i++;

                ModPackage modPackage;
                try
                {
                    modPackage = await ReadModPackageAsync(file).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    if (onException != null)
                    {
                        onException(i, e);
                        continue;
                    }

                    throw;
                }

                if (_fileSystem.Path.GetExtension(file) == ".pak")
                {
                    modPackage.Flags |= ModFlags.Enabled;
                }

                onPackageRead?.Invoke(i, modPackage);
                yield return modPackage;
            }
        }
    }

    public int CountModPackages(string path)
    {
        var patterns = new[] { "*.pak", "*.pak.off" };

        int i = 0;
        foreach (var pattern in patterns)
        {
            foreach (var _ in _fileSystem.Directory.EnumerateFiles(path, pattern, SearchOption.TopDirectoryOnly))
            {
                i++;
            }
        }

        return i;
    }

    private PackagedFile? FindMeta(Package pak)
    {
        foreach (var file in pak.Files)
        {
            var match = MetaRegex().Match(file.Name);
            if (match.Success && CommonAlteredGameFiles.All(gameFile => match.Groups[1].Value != gameFile))
            {
                return file;
            }
        }

        return null;
    }

    private PackagedFile? FindScriptExtenderConfig(Package pak)
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

    private ModFlags HasForceRecompile(Package pak) =>
        pak.Files.Select(file => ForceRecompileRegex().Match(file.Name)).Any(match => match.Success)
            ? ModFlags.HasForceRecompile
            : ModFlags.None;

    private ModFlags AltersGameFiles(Package pak) =>
        pak.Files.Any(file => file.Name.StartsWith("Video/") ||
                              CommonAlteredGameFiles.Any(f =>
                                  file.Name.StartsWith("Public/" + f) ||
                                  file.Name.StartsWith("Generated/Public/" + f) ||
                                  file.Name.StartsWith($"Mods/{f}/Public") ||
                                  file.Name.StartsWith($"Mods/{f}/Generated/Public")))
            ? ModFlags.AltersGameFiles
            : ModFlags.None;

    private ModFlags HasModFiles(Package pak)
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
