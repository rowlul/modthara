using System.Xml.Serialization;

namespace Modthara.Lari.Lsx;

[Serializable]
public class LsxRegion
{
    [XmlAttribute("id")]
    public required string Id { get; set; }

    [XmlElement("node")]
    public required LsxNode RootNode { get; set; }
}
