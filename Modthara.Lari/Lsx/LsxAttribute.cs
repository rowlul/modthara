using System.Xml.Serialization;

namespace Modthara.Lari.Lsx;

[Serializable]
public class LsxAttribute
{
    [XmlAttribute("id")]
    public required string Id { get; set; }

    [XmlAttribute("type")]
    public required string Type { get; set; }

    [XmlAttribute("value")]
    public required string Value { get; set; }
}
