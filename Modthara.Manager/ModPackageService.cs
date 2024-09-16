using System.IO.Abstractions;

using Modthara.Lari;
using Modthara.Lari.Lsx;
using Modthara.Lari.Pak;
using Modthara.Manager.Extensions;

using static Modthara.Manager.Constants;

namespace Modthara.Manager;

public class ModPackageService
{
    private readonly IFileSystem _fileSystem;

    public ModPackageService(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }


    public async ValueTask<ModPackage> ReadModPackageAsync(string path)
    {
        var stream = _fileSystem.FileStream.New(path, FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: StreamBufferSize, useAsync: true);

        try
        {
            var pak = await Task.Run(() => PackageReader.FromStream(stream)).ConfigureAwait(false);

            var t1 = Task.Run(() => pak.GetMetaLsxFile());
            var t2 = Task.Run(() => pak.GetModAdditionFlag());
            var t3 = Task.Run(() => pak.GetGameOverrideFlag());
            var t4 = Task.Run(() => pak.GetForceRecompileFlag());
            var t5 = Task.Run(() => pak.GetScriptExtenderFlag());

            await Task.WhenAll(t1, t2, t3, t4, t5).ConfigureAwait(false);

            var metaFile = t1.Result;
            var modAdditionFlag = t2.Result;
            var gameOverrideFlag = t3.Result;
            var forceRecompileFlag = t4.Result;
            var scriptExtenderFlag = t5.Result;

            var flags = modAdditionFlag | gameOverrideFlag | forceRecompileFlag | scriptExtenderFlag;

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
            await stream.DisposeAsync().ConfigureAwait(false);
        }
    }

    public async ValueTask<List<ModPackage>> ReadModPackagesAsync(string path)
    {
        List<ModPackage> packages = [];

        foreach (var file in _fileSystem.Directory.EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly))
        {
            if (file.EndsWith(".pak", StringComparison.OrdinalIgnoreCase) ||
                file.EndsWith(".pak.disabled", StringComparison.OrdinalIgnoreCase))
            {
                var package = await ReadModPackageAsync(file).ConfigureAwait(false);
                packages.Add(package);
            }
        }

        return packages;
    }
}
