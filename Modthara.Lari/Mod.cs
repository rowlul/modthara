using Modthara.Lari.Lsx;

namespace Modthara.Lari;

public class Mod
{
    public static Mod FromLsx(LsxDocument document)
    {
        var moduleInfoNode = document.FindNodeInRoot("Config", "ModuleInfo");
        var dependenciesNode = document.FindNodeInRoot("Config", "Dependencies");

        var mod = new Mod
        {
            Name = moduleInfoNode.GetAttributeValue("Name"),
            Author = moduleInfoNode.GetAttributeValue("Author", string.Empty),
            Description = moduleInfoNode.GetAttributeValue("Description", string.Empty),
            FolderName = moduleInfoNode.GetAttributeValue("Folder"),
            Md5 = moduleInfoNode.GetAttributeValue("Md5", string.Empty),
            Uuid = Guid.Parse(moduleInfoNode.GetAttributeValue("UUID")),
            Version = moduleInfoNode.GetVersion(),
            Dependencies = dependenciesNode.GetModules()
        };

        return mod;
    }

    public required string Name { get; set; }
    public string? Author { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public required string FolderName { get; set; }
    public string Md5 { get; set; } = string.Empty;
    public Guid Uuid { get; set; } = Guid.NewGuid();
    public LariVersion Version { get; set; } = 36028797018963968UL;
    public IList<Mod> Dependencies { get; set; } = [];
}
