using Modthara.Lari.Lsx;

namespace Modthara.Lari;

public class Mod
{
    public required string Name { get; set; }
    public string? Author { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public required string FolderName { get; set; }
    public string Md5 { get; set; } = string.Empty;
    public Guid Uuid { get; set; } = Guid.NewGuid();
    public LariVersion Version { get; set; } = 36028797018963968UL;
    public IList<Mod> Dependencies { get; set; } = [];

    public static Mod FromLsx(LsxDocument document)
    {
        var mod = document.FindNodeInRoot("Config", "ModuleInfo").ToMod();
        mod.Dependencies = document.FindNodeInRoot("Config", "Dependencies").GetModules();

        return mod;
    }

}
