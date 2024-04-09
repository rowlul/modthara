using System.Xml.Serialization;

namespace Modthara.Lari.Lsx;

/// <summary>
/// Represents <c>attribute</c> element in an <see cref="LsxNode"/>.
/// </summary>
[Serializable]
public class LsxAttribute
{
    [XmlAttribute("id")]
    public required string Id { get; set; }

    [XmlAttribute("value")]
    public required string Value { get; set; }

    [XmlAttribute("type")]
    public required string Type { get; set; }
}
