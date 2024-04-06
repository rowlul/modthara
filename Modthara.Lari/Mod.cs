using System.Xml;

namespace Modthara.Lari;

public class Mod
{
    public required string Name { get; set; }
    public required string Author { get; set; }
    public required string Description { get; set; }
    public required string FolderName { get; set; }
    public string Md5 { get; set; } = string.Empty;
    public Guid Uuid { get; set; } = Guid.NewGuid();
    public ModVersion Version { get; set; } = 36028797018963968UL;
    public IList<Mod> Dependencies { get; set; } = [];

    public static XmlDocument Document { get; } = new();

    public static Mod FromStream(Stream stream)
    {
        ValidateXml(stream);
        Document.Load(stream);

        var author = GetAttributeValue(SelectModuleAttribute("Author"));
        var description = GetAttributeValue(SelectModuleAttribute("Description"));
        var folder = GetAttributeValue(SelectModuleAttribute("Folder"));
        var name = GetAttributeValue(SelectModuleAttribute("Name"));
        var uuid = Guid.Parse(GetAttributeValue(SelectModuleAttribute("UUID")));
        var version = GetVersion();

        var mod = new Mod
        {
            Author = author,
            Description = description,
            FolderName = folder,
            Name = name,
            Uuid = uuid,
            Version = version
        };

        return mod;
    }

    private static string GetAttributeAddress(string nodeId, string attributeId) =>
        $"/save/region[@id='Config']/node[@id='root']/children/node[@id='{nodeId}']/attribute[@id='{attributeId}']";

    private static XmlNode SelectModuleAttribute(string id) =>
        Document.SelectSingleNode(GetAttributeAddress(ModuleInfoNode, id)) ??
        throw new LsxMarkupException($"Required attribute '{id}' does not exist.");

    private static string GetAttributeValue(XmlNode attribute)
    {
        if (attribute.Attributes == null || string.IsNullOrEmpty(attribute.Attributes["value"]!.Value))
        {
            throw new LsxMarkupException($"No value for attribute '{attribute.Name}'.");
        }

        return attribute.Attributes["value"]!.Value;
    }

    private static ModVersion GetVersion()
    {
        var node = Document.SelectSingleNode(GetAttributeAddress(ModuleInfoNode, "Version64")) ??
                   Document.SelectSingleNode(GetAttributeAddress(ModuleInfoNode, "Version32")) ??
                   Document.SelectSingleNode(GetAttributeAddress(ModuleInfoNode, "Version")) ??
                   throw new LsxMarkupException("Version attribute does not exist.");

        var version = ModVersion.FromUint64(Convert.ToUInt64(GetAttributeValue(node)));
        return version;
    }

    private static void ValidateXml(Stream stream)
    {
        using var reader = XmlReader.Create(stream);
        try
        {
            while (reader.Read())
            {
            }
        }
        catch (XmlException e)
        {
            var info = (IXmlLineInfo)reader;
            throw new LsxMarkupException("Stream has invalid XML data.", (info.LineNumber, info.LinePosition), e);
        }
        finally
        {
            stream.Position = 0;
        }
    }

    private const string ModuleInfoNode = "ModuleInfo";
}
