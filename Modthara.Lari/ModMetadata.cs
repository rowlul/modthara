using Modthara.Lari.Extensions;
using Modthara.Lari.Lsx;
using Modthara.Lari.Lsx.Factories;

namespace Modthara.Lari;

/// <summary>
/// Represents <c>Config</c> region.
/// </summary>
public sealed class ModMetadata : Module
{
    private readonly LsxDocument _document;

    private LsxNode ModuleInfoNode => _document.GetRegion("Config").GetNode("ModuleInfo");
    private LsxNode? DependenciesNode => _document.GetRegion("Config").GetNodeOrDefault("Dependencies");

    public string Author { get; set; }
    public string Description { get; set; }
    public IReadOnlyList<Module>? Dependencies { get; set; }

    public ModMetadata(LsxDocument document) : base(document.GetRegion("Config").GetNode("ModuleInfo"))
    {
        _document = document;

        Author = ModuleInfoNode.GetAttribute("Author").Value;
        Description = ModuleInfoNode.GetAttribute("Description").Value;

        if (DependenciesNode is { Children: not null })
        {
            Dependencies = DependenciesNode.Children.ToShortDescModules();
        }
    }

    public ModMetadata(LariUuid uuid, string folderName, LariVersion version, string md5, string name, string author,
        string description, ICollection<Module>? dependencies = null) : this(
        LsxDocumentFactory.CreateMeta(new LsxModuleAttributes(uuid, folderName, md5, name, version, author, description,
            dependencies)))
    {
    }

    public new LsxNode ToNode() => ModuleInfoNode;

    public LsxDocument ToDocument() => _document;
}
