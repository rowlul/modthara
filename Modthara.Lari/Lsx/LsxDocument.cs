using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Modthara.Lari.Lsx;

/// <summary>
/// Represents an arbitrary document in LSX format.
/// </summary>
[Serializable]
[XmlRoot("save")]
public class LsxDocument
{
    [XmlElement("version")]
    public required LariVersion Version { get; set; }

    [XmlElement("region")]
    public required List<LsxRegion> Regions { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="LsxDocument"/> by deserializing <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <returns>
    /// Instance of <see cref="LsxDocument"/>. Returns null if serialized document was null.
    /// </returns>
    public static LsxDocument FromStream(Stream stream)
    {
        var serializer = new XmlSerializer(typeof(LsxDocument));
        var document = (LsxDocument)serializer.Deserialize(stream)!;
        return document;
    }

    /// <summary>
    /// Serializes instance to <see cref="Stream"/>.
    /// </summary>
    /// <returns>Returns stream containing the serialized document.</returns>
    public Stream ToStream()
    {
        var serializer = new XmlSerializer(typeof(LsxDocument));
        var stream = new MemoryStream();

        var writer = new LsxTextWriter(stream);
        serializer.Serialize(writer, this,
            new XmlSerializerNamespaces([new XmlQualifiedName(string.Empty, string.Empty)]));

        stream.Position = 0;
        return stream;
    }

    /// <summary>
    /// Traverses through the region and its root node to find the specified node.
    /// </summary>
    /// <param name="regionId">Region id to look for.</param>
    /// <param name="nodeId">Node id to look for.</param>
    /// <returns>Found instance of <see cref="LsxNode"/>.</returns>
    /// <exception cref="LsxMissingElementException">Throws if region or node were not found.</exception>
    public LsxNode FindNodeInRoot(string regionId, string nodeId)
    {
        var configRegion = this.Regions.FirstOrDefault(r => r.Id == regionId);
        if (configRegion?.RootNode is { Id: "root", Children: not null })
        {
            return configRegion.RootNode.Children.FirstOrDefault(n => n.Id == nodeId)
                   ?? throw new LsxMissingElementException(regionId);
        }

        throw new LsxMissingElementException(regionId);
    }
}
