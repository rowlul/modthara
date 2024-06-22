﻿using Modthara.Essentials.Packaging;
using Modthara.Lari;

namespace Modthara.App.ViewModels;

public partial class ModPackageViewModel : ViewModelBase
{
    private readonly ModPackage _modPackage;
    private readonly IModsService _modsService;

    public ModPackageViewModel(ModPackage modPackage, IModsService modsService)
    {
        _modPackage = modPackage;
        _modsService = modsService;
    }

    public string Path
    {
        get => _modPackage.Path;
        set
        {
            _modPackage.Path = value;
            OnPropertyChanged();
        }
    }

    public ModFlags Flags
    {
        get => _modPackage.Flags;
        set
        {
            _modPackage.Flags = value;
            OnPropertyChanged();
        }
    }

    public DateTime LastModified
    {
        get => _modPackage.LastModified;
        set
        {
            _modPackage.LastModified = value;
            OnPropertyChanged();
        }
    }

    public string Name
    {
        get => _modPackage.Name;
        set
        {
            _modPackage.Name = value;
            OnPropertyChanged();
        }
    }

    public string? Author
    {
        get => _modPackage.Author;
        set
        {
            _modPackage.Author = value;
            OnPropertyChanged();
        }
    }

    public string? Description
    {
        get => _modPackage.Description;
        set
        {
            _modPackage.Description = value;
            OnPropertyChanged();
        }
    }

    public string Md5
    {
        get => _modPackage.Md5;
        set
        {
            _modPackage.Md5 = value;
            OnPropertyChanged();
        }
    }

    public string FolderName
    {
        get => _modPackage.FolderName;
        set
        {
            _modPackage.FolderName = value;
            OnPropertyChanged();
        }
    }

    public IList<ModMetadata>? Dependencies
    {
        get => _modPackage.Dependencies;
        set
        {
            _modPackage.Dependencies = value;
            OnPropertyChanged();
        }
    }

    public LariUuid Uuid
    {
        get => _modPackage.Uuid;
        set
        {
            _modPackage.Uuid = value;
            OnPropertyChanged();
        }
    }

    public LariVersion Version
    {
        get => _modPackage.Version;
        set
        {
            _modPackage.Version = value;
            OnPropertyChanged();
        }
    }

    public bool HasOverridesExclusively =>
        Flags.HasFlag(ModFlags.AltersGameFiles) && !Flags.HasFlag(ModFlags.HasModFiles);

    public bool HasAnyStandaloneFiles => Flags.HasFlag(ModFlags.HasModFiles);

    public ModPackage AsModPackage() => _modPackage;

    public void Enable()
    {
        _modsService.EnableModPackage(_modPackage);
        OnPropertyChanged(nameof(Flags));
    }

    public void Disable()
    {
        _modsService.DisableModPackage(_modPackage);
        OnPropertyChanged(nameof(Flags));
    }

    public void Delete()
    {
        _modsService.DeleteModPackage(_modPackage);
    }
}
