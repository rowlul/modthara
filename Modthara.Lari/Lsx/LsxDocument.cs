using System.Xml.Serialization;

namespace Modthara.Lari.Lsx;

[Serializable]
[XmlRoot("save")]
public class LsxDocument
{
    [XmlElement("version")] public required LariVersion Version { get; set; }
    [XmlElement("region")] public required List<LsxRegion> Regions { get; set; }

    public static LsxDocument? FromStream(Stream stream)
    {
        var serializer = new XmlSerializer(typeof(LsxDocument));
        var document = (LsxDocument?)serializer.Deserialize(stream);
        return document;
    }
}
