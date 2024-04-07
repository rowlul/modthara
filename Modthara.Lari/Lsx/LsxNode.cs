﻿using System.Xml.Serialization;

namespace Modthara.Lari.Lsx;

[Serializable]
public class LsxNode
{
    [XmlAttribute("id")] public required string Id { get; set; }

    [XmlArray("children")]
    [XmlArrayItem("node")]
    public List<LsxNode>? Children { get; set; }

    [XmlElement("attribute")] public List<LsxAttribute>? Attributes { get; set; }

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

        throw new LsxMarkupException($"Attribute with id '{attributeId}' is missing, null, or empty.");
    }

    public LariVersion GetVersion()
    {
        var value = this.Attributes?.FirstOrDefault(n => n.Id is "Version64" or "Version32" or "Version")?.Value;
        if (value == null)
        {
            throw new LsxMarkupException("Version attribute is missing, null, or empty.");
        }

        return Convert.ToUInt64(value);
    }

    public IList<Mod> GetModules()
    {
        if (this.Children == null)
        {
            return [];
        }

        List<Mod> modules = [];
        modules.AddRange(this.Children.Where(d => d.Id == "ModuleShortDesc")
            .Select(module => new Mod
            {
                FolderName = module.GetAttributeValue("Folder"),
                Md5 = module.GetAttributeValue("Md5", string.Empty),
                Name = module.GetAttributeValue("Name"),
                Uuid = Guid.Parse(module.GetAttributeValue("UUID")),
                Version = module.GetVersion()
            }));

        return modules;
    }
}
