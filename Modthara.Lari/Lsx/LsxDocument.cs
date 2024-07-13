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
    /// Deserializes an <see cref="LsxDocument"/> from the specified stream.
    /// </summary>
    /// <param name="stream">The stream containing the serialized <see cref="LsxDocument"/>.</param>
    /// <returns>The deserialized <see cref="LsxDocument"/>.</returns>
    public static LsxDocument FromStream(Stream stream)
    {
        var serializer = new XmlSerializer(typeof(LsxDocument));
        var document = (LsxDocument)serializer.Deserialize(stream)!;
        return document;
    }

    /// <summary>
    /// Serializes the current <see cref="LsxDocument"/> to a stream.
    /// </summary>
    /// <returns>A stream containing the serialized <see cref="LsxDocument"/>.</returns>
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
    /// Gets the region with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the region to retrieve.</param>
    /// <returns>The region with the specified ID.</returns>
    /// <exception cref="LsxMissingElementException">
    /// Thrown when a region with the specified ID is not found.
    /// </exception>
    public LsxRegion GetRegion(string id) =>
        this.Regions.FirstOrDefault(r => r.Id == id) ?? throw new LsxMissingElementException(id);
}
