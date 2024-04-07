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

    public LsxNode FindNodeInRoot(string regionId, string nodeId)
    {
        var configRegion = this.Regions.FirstOrDefault(r => r.Id == regionId);
        if (configRegion?.RootNode is { Id: "root", Children: not null })
        {
            return configRegion.RootNode.Children.FirstOrDefault(n => n.Id == nodeId)
                   ?? throw new LsxMarkupException($"Could not find required node '{nodeId}'.");
        }

        throw new LsxMarkupException($"Could not find required region '{regionId}'.");
    }
}
