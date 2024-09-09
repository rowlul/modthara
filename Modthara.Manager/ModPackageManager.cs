using System.Diagnostics;
using System.IO.Abstractions;
using System.Text.RegularExpressions;

using Modthara.Lari;
using Modthara.Lari.Lsx;
using Modthara.Lari.Pak;

using static Modthara.Manager.Constants;

namespace Modthara.Manager;

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
            bufferSize: StreamBufferSize, useAsync: true);

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
            var t5 = Task.Run(() => RequiresScriptExtender(pak));

            await Task.WhenAll(t1, t2, t3, t4, t5).ConfigureAwait(false);

            var metaFile = t1.Result;
            var hasModFiles = t2.Result;
            var altersGameFiles = t3.Result;
            var hasForceRecompile = t4.Result;
            var requiresScriptExtender = t5.Result;

            var flags = hasModFiles | altersGameFiles | hasForceRecompile | requiresScriptExtender;

            var lastModified = _fileSystem.FileInfo.New(path).LastWriteTime;

            ModMetadata? meta = null;
            if (metaFile != null)
            {
                var metaStream = metaFile.Open();
                var lsx = await Task.Run(() => LsxDocument.FromStream(metaStream)).ConfigureAwait(false);
                meta = await Task.Run(() => new ModMetadata(lsx)).ConfigureAwait(false);
                await metaStream.DisposeAsync().ConfigureAwait(false);
            }

            var name = meta != null ? meta.Name : _fileSystem.Path.GetFileNameWithoutExtension(path);

            var modPackage = new ModPackage
            {
                Path = path,
                Name = name,
                Package = pak,
                Metadata = meta,
                Flags = flags,
                LastModified = lastModified,
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

    public async ValueTask<List<ModPackage>> ReadModPackagesAsync(string path,
        Action<int, Exception>? onException = null,
        Action<int, ModPackage>? onPackageRead = null)
    {
        List<ModPackage> packages = [];

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
                    if (e is LspkException ex && ex.Version != 18)
                    {
                        Debug.WriteLine($"Skipping {Path.GetFileName(file)} with old LSPK: {ex.Version}");
                        continue;
                    }

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
                packages.Add(modPackage);
            }
        }

        return packages;
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

    private ModFlags RequiresScriptExtender(Package pak) =>
        pak.Files.Select(file => ScriptExtenderConfigRegex().Match(file.Name)).Any(match => match.Success)
            ? ModFlags.RequiresScriptExtender
            : ModFlags.None;

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
            var hasScripts = file.Name.StartsWith("Scripts/");
            if (hasScripts)
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
