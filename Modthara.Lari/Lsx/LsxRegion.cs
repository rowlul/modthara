using System.Xml.Serialization;

namespace Modthara.Lari.Lsx;

/// <summary>
/// Represents <c>region</c> element in an <see cref="LsxDocument"/>.
/// </summary>
[Serializable]
public class LsxRegion
{
    [XmlAttribute("id")]
    public required string Id { get; set; }

    [XmlElement("node")]
    public required LsxNode RootNode { get; set; }
}
