using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;

using Humanizer;

using Modthara.Manager;

namespace Modthara.UI.ViewModels;

public partial class ModPackageViewModel : ViewModelBase
{
    public readonly ModPackage _modPackage;

    [ObservableProperty]
    private bool _isEnabled;

    partial void OnIsEnabledChanged(bool value)
    {
        if (value)
        {
            Enable();
        }
        else
        {
            Disable();
        }
    }

    private IModsService ModsService { get; } = Ioc.Default.GetRequiredService<IModsService>();

    public bool HasOverrides => (_modPackage.Flags & ModFlags.AltersGameFiles) != ModFlags.None;
    public bool HasModFiles => (_modPackage.Flags & ModFlags.HasModFiles) != ModFlags.None;

    public bool IsModWithOverrides => HasModFiles && HasOverrides;
    public bool IsPureOverride => HasOverrides && !HasModFiles;

    #region Mod Package Properties

    public string Name => _modPackage.Name;
    public string? Author => _modPackage.Metadata?.Author;
    public string? Description => _modPackage.Metadata?.Description;
    public string? Version => _modPackage.Metadata?.Version.ToString();
    public string LastModified => _modPackage.LastModified.Humanize();

    #endregion

    public ModPackageViewModel(ModPackage modPackage)
    {
        _modPackage = modPackage;

        _isEnabled = (_modPackage.Flags & ModFlags.Enabled) != ModFlags.None;
    }

    public void Enable()
    {
        ModsService.EnableModPackage(_modPackage);
        IsEnabled = true;
    }

    public void Disable()
    {
        ModsService.DisableModPackage(_modPackage);
        IsEnabled = false;
    }
}
