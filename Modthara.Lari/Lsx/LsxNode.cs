using System.Xml.Serialization;

namespace Modthara.Lari.Lsx;

/// <summary>
/// Represents <c>node</c> element in an <see cref="LsxDocument"/>.
/// </summary>
[Serializable]
public class LsxNode
{
    [XmlAttribute("id")]
    public required string Id { get; set; }

    [XmlArray("children")]
    [XmlArrayItem("node")]
    public List<LsxNode>? Children { get; set; }

    [XmlElement("attribute")]
    public List<LsxAttribute>? Attributes { get; set; }

    public bool ShouldSerializeChildren()
    {
        return Children != null && Children.Count != 0;
    }

    public bool ShouldSerializeAttributes()
    {
        return Attributes != null && Attributes.Count != 0;
    }

    /// <summary>
    /// Traverses through the node to find specified attribute.
    /// </summary>
    /// <param name="attributeId">Attribute id to look for.</param>
    /// <param name="defaultValue">Fallback value if attribute was null.</param>
    /// <returns>String value of the attribute.</returns>
    /// <exception cref="LsxMissingElementException">
    /// Throws if attribute was null unless <paramref name="defaultValue"/> is provided.
    /// </exception>
    public string GetAttributeValue(string attributeId, string? defaultValue = null)
    {
        var value = this.Attributes?.FirstOrDefault(n => n.Id == attributeId)?.Value;
        if (!string.IsNullOrEmpty(value))
        {
            return value;
        }
        else if (defaultValue != null)
        {
            return defaultValue;
        }

        throw new LsxMissingElementException(attributeId);
    }

    /// <summary>
    /// Gets UUID value of the respective attribute.
    /// </summary>
    /// <returns>UUID of the node.</returns>
    public Guid GetUuid() => Guid.Parse(this.GetAttributeValue("UUID"));

    public LariVersion GetVersion()
    {
        var value = this.Attributes?.FirstOrDefault(n => n.Id is "Version64" or "Version32" or "Version")?.Value;
        if (value == null)
        {
            throw new LsxMissingElementException("Version");
        }

        return Convert.ToUInt64(value);
    }

    /// <summary>
    /// Gets a list of <c>ModuleShortDesc</c> nodes.
    /// </summary>
    /// <returns>List of mods.</returns>
    public List<Mod> GetModules()
    {
        if (this.Children == null)
        {
            return [];
        }

        List<Mod> modules = [];
        modules.AddRange(this.Children.Where(d => d.Id == "ModuleShortDesc")
            .Select(module => module.ToMod()));

        return modules;
    }

    /// <summary>
    /// Converts node to <see cref="Mod"/>.
    /// </summary>
    /// <returns>Instance of <see cref="Mod"/>.</returns>
    public Mod ToMod()
    {
        var mod = new Mod
        {
            Name = this.GetAttributeValue("Name"),
            Author = this.GetAttributeValue("Author", string.Empty),
            Description = this.GetAttributeValue("Description", string.Empty),
            FolderName = this.GetAttributeValue("Folder"),
            Md5 = this.GetAttributeValue("Md5", string.Empty),
            Uuid = this.GetUuid(),
            Version = this.GetVersion(),
        };

        return mod;
    }
}
