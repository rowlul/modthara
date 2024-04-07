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
            Name = GetAttributeValue(moduleInfoNode, "Name") ?? ThrowIfNull("Name"),
            Author = GetAttributeValue(moduleInfoNode, "Author") ?? string.Empty,
            Description = GetAttributeValue(moduleInfoNode, "Description") ?? string.Empty,
            FolderName = GetAttributeValue(moduleInfoNode, "Folder") ?? ThrowIfNull("Folder"),
            Md5 = GetAttributeValue(moduleInfoNode, "Md5") ?? string.Empty,
            Uuid = Guid.Parse(GetAttributeValue(moduleInfoNode, "UUID") ?? ThrowIfNull("UUID")),
            Version = GetVersion(moduleInfoNode),
            Dependencies = GetDependencies(dependenciesNode),
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

    private static string? GetAttributeValue(LsxNode node, string attributeId)
    {
        var value = node.Attributes?.FirstOrDefault(n => n.Id == attributeId)?.Value;
        return value;
    }

    private static string ThrowIfNull(string attributeName)
    {
        throw new LsxMarkupException($"Required attribute '{attributeName}' is missing, null, or empty.");
    }

    private static LariVersion GetVersion(LsxNode node)
    {
        var value = node.Attributes?.FirstOrDefault(n => n.Id is "Version64" or "Version32" or "Version")?.Value ??
                    ThrowIfNull("Version64");
        return Convert.ToUInt64(value);
    }

    private static List<Mod> GetDependencies(LsxNode dependenciesNode)
    {
        if (dependenciesNode.Children != null)
        {
            List<Mod> dependencies = [];
            dependencies.AddRange(dependenciesNode.Children.Where(d => d.Id == "ModuleShortDesc")
            .Select(d => new Mod
            {
                FolderName = GetAttributeValue(d, "Folder") ?? ThrowIfNull("Folder"),
                Md5 = GetAttributeValue(d, "Md5") ?? string.Empty,
                Name = GetAttributeValue(d, "Name") ?? ThrowIfNull("Name"),
                Uuid = Guid.Parse(GetAttributeValue(d, "UUID") ?? ThrowIfNull("UUID")),
                Version = GetVersion(d)
            }));
            return dependencies;
        }

        return [];
    }
}
