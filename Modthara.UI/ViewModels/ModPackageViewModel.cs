using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;

using Humanizer;

using Modthara.Manager;

namespace Modthara.UI.ViewModels;

public partial class ModPackageViewModel : ViewModelBase
{
    private ModManager ModManager { get; } = Ioc.Default.GetRequiredService<ModManager>();
    private PackagesViewModel PackagesViewModel { get; } = Ioc.Default.GetRequiredService<PackagesViewModel>();

    private readonly ModPackage _modPackage;

    [ObservableProperty]
    private bool _isEnabled;

    async partial void OnIsEnabledChanged(bool value)
    {
        if (value)
        {
            EnablePackage();
            EnableModule();
        }
        else
        {
            DisableModule();
            DisablePackage();
        }

        await ModManager.SaveModSettingsAsync();

        // delay prevents visual bug with DataGridCollectionView
        await Task.Delay(10);
        // TODO: decouple by using IMessenger
        PackagesViewModel.MoveAfterEnabledMods(this);
    }

    public bool IsGameOverride => _modPackage.Flags.HasFlag(ModFlags.GameOverride);
    public bool IsModAddition => _modPackage.Flags.HasFlag(ModFlags.ModAddition);
    public bool RequiresScriptExtender => _modPackage.Flags.HasFlag(ModFlags.ScriptExtender);
    public bool HasForceRecompile => _modPackage.Flags.HasFlag(ModFlags.ForceRecompile);
    public bool HasMetadata => _modPackage.Metadata != null;

    #region Mod Package Properties

    public string Path => _modPackage.Path;
    public string Name => _modPackage.Name;
    public string? Author => _modPackage.Metadata?.Author;
    public string? Description => _modPackage.Metadata?.Description;
    public string? Version => _modPackage.Metadata?.Version.ToString();
    public string LastModified => _modPackage.LastModified.Humanize();

    #endregion

    public ModPackageViewModel(ModPackage modPackage)
    {
        _modPackage = modPackage;

        _isEnabled = ModManager.IsModuleEnabled(modPackage.Metadata) && ModManager.IsPackageEnabled(modPackage);
    }

    public void EnablePackage() => ModManager.EnablePackage(_modPackage);

    public void EnableModule() => ModManager.EnableModule(_modPackage.Metadata);

    public void DisableModule() => ModManager.DisableModule(_modPackage.Metadata);

    public void DisablePackage() => ModManager.DisablePackage(_modPackage);

    public void DeleteModPackage() => ModManager.DeleteModPackage(_modPackage);
}
