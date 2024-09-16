namespace Modthara.Manager;

using static Path;

public interface IPathProvider
{
    string LocalRoot { get; }

    string ModsFolder => LocalRoot + DirectorySeparatorChar + "Baldur's Gate 3" + DirectorySeparatorChar + "Mods";

    string ModSettingsFile => LocalRoot + DirectorySeparatorChar + "Baldur's Gate 3" + DirectorySeparatorChar +
                              "PlayerProfiles" + DirectorySeparatorChar + "Public" + DirectorySeparatorChar +
                              "modsettings.lsx";
}
