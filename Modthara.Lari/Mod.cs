using Modthara.Lari.Lsx;

namespace Modthara.Lari;

public class Mod
{
    public static Mod FromLsx(LsxDocument document)
    {
        var moduleInfoNode = LocateModuleInfo(document);

        var mod = new Mod
        {
            Name = GetAttributeValue(moduleInfoNode, "Name") ?? ThrowMissing("Name"),
            Author = GetAttributeValue(moduleInfoNode, "Author") ?? ThrowMissing("Author"),
            Description = GetAttributeValue(moduleInfoNode, "Description") ?? ThrowMissing("Description"),
            FolderName = GetAttributeValue(moduleInfoNode, "Folder") ?? ThrowMissing("Folder"),
            Md5 = GetAttributeValue(moduleInfoNode, "Md5") ?? string.Empty,
            Uuid = Guid.Parse(GetAttributeValue(moduleInfoNode, "UUID") ?? ThrowMissing("UUID")),
            Version = GetVersion(moduleInfoNode)
        };

        return mod;
    }

    public required string Name { get; set; }
    public required string Author { get; set; }
    public required string Description { get; set; }
    public required string FolderName { get; set; }
    public string Md5 { get; set; } = string.Empty;
    public Guid Uuid { get; set; } = Guid.NewGuid();
    public LariVersion Version { get; set; } = 36028797018963968UL;
    public IList<Mod> Dependencies { get; set; } = [];

    private static LsxNode LocateModuleInfo(LsxDocument document)
    {
        var configRegion = document.Regions.FirstOrDefault(r => r.Id == "Config");
        if (configRegion?.RootNode is { Id: "root", Children: not null })
        {
            return configRegion.RootNode.Children.FirstOrDefault(n => n.Id == "ModuleInfo")
                   ?? throw new LsxMarkupException("Could not find required node 'ModuleInfo'.");
        }

        throw new LsxMarkupException("Could not find required region 'Config'.");
    }

    private static string? GetAttributeValue(LsxNode node, string attributeId)
    {
        var value = node.Attributes?.FirstOrDefault(n => n.Id == attributeId)?.Value;
        return value;
    }

    private static string ThrowMissing(string attributeName)
    {
        throw new LsxMarkupException($"Required attribute '{attributeName}' is missing, null, or empty.");
    }

    private static LariVersion GetVersion(LsxNode node)
    {
        var value = node.Attributes?.FirstOrDefault(n => n.Id is "Version64" or "Version32" or "Version")?.Value ??
                    ThrowMissing("Version64");
        return Convert.ToUInt64(value);
    }
}
