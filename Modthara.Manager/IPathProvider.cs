namespace Modthara.Manager;

public interface IPathProvider
{
    string LocalLarianFolder { get; }

    string LocalGameFolder => Path.Combine(LocalLarianFolder, "Baldur's Gate 3");

    string ModsFolder => Path.Combine(LocalGameFolder, "Mods");

    string ProfileFolder => Path.Combine(LocalGameFolder, "PlayerProfiles", "Public");

    string ModSettingsFile => Path.Combine(ProfileFolder, "modsettings.lsx");
}
