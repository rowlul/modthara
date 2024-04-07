using System.Xml.Serialization;

namespace Modthara.Lari.Lsx;

[Serializable]
public class LsxNode
{
    [XmlAttribute("id")] public required string Id { get; set; }

    [XmlArray("children")]
    [XmlArrayItem("node")]
    public List<LsxNode>? Children { get; set; }

    [XmlElement("attribute")]
    public List<LsxAttribute>? Attributes { get; set; }
}
