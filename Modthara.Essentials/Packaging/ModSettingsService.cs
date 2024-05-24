using System.IO.Abstractions;

using Modthara.Lari;
using Modthara.Lari.Lsx;

namespace Modthara.Essentials.Packaging;

/// <inheritdoc />
public class ModSettingsService : IModSettingsService
{
    private readonly string _path;

    private readonly IFileSystem _fileSystem;

    /// <inheritdoc />
    public ModSettings? ModSettings { get; private set; }

    public ModSettingsService(string path, IFileSystem fileSystem)
    {
        _path = path;

        _fileSystem = fileSystem;
    }

    /// <inheritdoc />
    public async Task LoadModSettingsAsync()
    {
        var file = _fileSystem.FileStream.New(_path, FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: 4096, useAsync: true);
        var lsx = await Task.Run(() => LsxDocument.FromStream(file)).ConfigureAwait(false);
        await file.DisposeAsync().ConfigureAwait(false);

        var modSettings = new ModSettings(lsx);
        await Task.Run(() => modSettings.Sanitize()).ConfigureAwait(false);

        ModSettings = modSettings;
    }

    /// <inheritdoc />
    public async Task SaveModSettingsAsync()
    {
        if (ModSettings == null)
        {
            throw new InvalidOperationException($"Instance of {nameof(ModSettings)} was null.");
        }

        await using var stream = ModSettings.ToStream();
        await using var file = _fileSystem.FileStream.New(_path, FileMode.Create, FileAccess.Write, FileShare.None,
            bufferSize: 4096, useAsync: true);
        await stream.CopyToAsync(file).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public IEnumerable<ModPackage> GetMods(IReadOnlyList<ModPackage> modPackages,
        Action<int, Exception>? onException = null, Action<int, ModPackage>? onPackageRead = null)
    {
        if (ModSettings == null)
        {
            throw new InvalidOperationException($"Instance of {nameof(ModSettings)} was null.");
        }

        for (var i = 0; i < ModSettings.Mods.Count; i++)
        {
            var modMeta = ModSettings.Mods[i];

            ModPackage? modPackage = null;

            try
            {
                foreach (var m in modPackages)
                {
                    if (modMeta.Uuid.Value == m.Uuid.Value)
                    {
                        modPackage = m;
                    }
                    else
                    {
                        throw new ModNotFoundException($"Mod not found in mod settings: {m.Uuid.Value}", m.Uuid);
                    }
                }
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

            if (modPackage != null)
            {
                onPackageRead?.Invoke(i, modPackage);
                yield return modPackage;
            }
        }
    }
}
