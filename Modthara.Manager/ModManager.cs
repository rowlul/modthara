using System.IO.Abstractions;

using Modthara.Lari;
using Modthara.Manager.Extensions;

namespace Modthara.Manager;

public class ModManager
{
    private readonly IPathProvider _pathProvider;
    private readonly IFileSystem _fileSystem;
    private readonly ModPackageService _modPackageService;
    private readonly ModSettingsService _modSettingsService;

    private ModSettings _modSettings = null!;
    private List<ModPackage> _modPackages = null!;

    public IReadOnlyList<Module> Modules => _modSettings.Modules;

    public IReadOnlyList<ModPackage> ModPackages => _modPackages;

    public ModManager(IPathProvider pathProvider, IFileSystem fileSystem, ModPackageService modPackageService,
        ModSettingsService modSettingsService)
    {
        _pathProvider = pathProvider;
        _fileSystem = fileSystem;
        _modPackageService = modPackageService;
        _modSettingsService = modSettingsService;
    }

    public async Task LoadModsAsync()
    {
        var modSettings =
            await _modSettingsService.ReadModSettingsAsync(_pathProvider.ModSettingsFile).ConfigureAwait(false);
        var modPackages =
            await _modPackageService.ReadModPackagesAsync(_pathProvider.ModsFolder).ConfigureAwait(false);
        _modSettings = modSettings;
        _modPackages = modPackages;
    }

    public (List<ModPackage> found, List<Module> missing) MatchMods()
    {
        List<ModPackage> found = [];
        List<Module> missing = [];

        foreach (var module in _modSettings.Modules)
        {
            if (module.IsGameFile())
            {
                continue;
            }

            bool isFound = false;
            foreach (var modPackage in _modPackages)
            {
                if (modPackage.Metadata == null)
                {
                    continue;
                }

                if (module.Uuid.Value == modPackage.Metadata.Uuid.Value)
                {
                    found.Add(modPackage);
                    isFound = true;
                    break;
                }
            }

            if (!isFound)
            {
                missing.Add(module);
            }
        }

        return (found, missing);
    }

    public List<(ModPackage, ModPackage)> FindDuplicateModPackages()
    {
        var duplicates = new List<(ModPackage, ModPackage)>();

        for (int i = 0; i < _modPackages.Count; i++)
        {
            for (int j = i + 1; j < _modPackages.Count; j++)
            {
                if (_modPackages[i].Metadata?.Uuid == _modPackages[j].Metadata?.Uuid)
                {
                    duplicates.Add((_modPackages[i], _modPackages[j]));
                }
            }
        }

        return duplicates;
    }

    public void MoveModule(int oldIndex, int newIndex) => _modSettings.Move(oldIndex + 1, newIndex + 1);

    public bool IsPackageEnabled(ModPackage modPackage) =>
        _modPackages.Any(x => x.Equals(modPackage) && !x.Path.EndsWith(".disabled"));

    public bool IsModuleEnabled(Module? module) =>
        module == null || _modSettings.Modules.Any(x => x.Uuid == module.Uuid);

    public void EnablePackage(ModPackage modPackage)
    {
        if (!IsPackageEnabled(modPackage))
        {
            var newPath = _fileSystem.Path.ChangeExtension(modPackage.Path, null);
            _fileSystem.FileInfo.New(modPackage.Path).MoveTo(newPath);

            modPackage.Path = newPath;
        }
    }

    public void DisablePackage(ModPackage modPackage)
    {
        if (IsPackageEnabled(modPackage))
        {
            var newPath = _fileSystem.Path.ChangeExtension(modPackage.Path, ".pak.disabled");
            _fileSystem.FileInfo.New(modPackage.Path).MoveTo(newPath);

            modPackage.Path = newPath;
        }
    }

    public void EnableModule(Module? module, int index = -1)
    {
        if (module != null && !IsModuleEnabled(module))
        {
            if (index >= 0)
            {
                _modSettings.Insert(index, module);
            }
            else
            {
                _modSettings.Append(module);
            }
        }
    }

    public void DisableModule(Module? module)
    {
        if (module != null && IsModuleEnabled(module))
        {
            _modSettings.Remove(module);
        }
    }

    public void DeleteModPackage(ModPackage modPackage)
    {
        DisableModule(modPackage.Metadata);

        _modPackages.Remove(modPackage);
        _fileSystem.File.Delete(modPackage.Path);
    }

    public void ClearModSettings()
    {
        var gustavDev = Modules[0];
        _modSettings.Clear();
        _modSettings.Append(gustavDev);
    }

    public async Task SaveModSettingsAsync()
    {
        await _modSettingsService.WriteModSettingsAsync(_modSettings, _pathProvider.ModSettingsFile)
            .ConfigureAwait(false);
    }
}
