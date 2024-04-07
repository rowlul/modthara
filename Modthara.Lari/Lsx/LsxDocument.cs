using System.Xml.Serialization;

namespace Modthara.Lari.Lsx;

[Serializable]
[XmlRoot("save")]
public class LsxDocument
{
    [XmlElement("version")] public required LariVersion Version { get; set; }
    [XmlElement("region")] public required List<LsxRegion> Regions { get; set; }
}
