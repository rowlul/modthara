using System.Xml.Serialization;

namespace Modthara.Lari.Lsx;

/// <summary>
/// Represents <c>node</c> element in an <see cref="LsxDocument"/>.
/// </summary>
[Serializable]
public class LsxNode
{
    [XmlAttribute("id")]
    public required string Id { get; set; }

    [XmlArray("children")]
    [XmlArrayItem("node")]
    public List<LsxNode>? Children { get; set; }

    [XmlElement("attribute")]
    public List<LsxAttribute>? Attributes { get; set; }

    public bool ShouldSerializeChildren()
    {
        return Children != null && Children.Count != 0;
    }

    public bool ShouldSerializeAttributes()
    {
        return Attributes != null && Attributes.Count != 0;
    }

    /// <summary>
    /// Gets the attribute with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the attribute to retrieve.</param>
    /// <returns>The attribute with the specified ID.</returns>
    /// <exception cref="LsxMissingElementException">
    /// Thrown when an attribute with the specified ID is not found.
    /// </exception>
    public LsxAttribute GetAttribute(string id) =>
        this.Attributes?.FirstOrDefault(x => x.Id == id) ?? throw new LsxMissingElementException(id);

    /// <summary>
    /// Gets the attribute with the specified ID, or null if no such attribute exists.
    /// </summary>
    /// <param name="id">The ID of the attribute to retrieve.</param>
    /// <returns>The attribute with the specified ID, or null if no such attribute exists.</returns>
    public LsxAttribute? GetAttributeOrDefault(string id) =>
        this.Attributes?.FirstOrDefault(x => x.Id == id);

    /// <summary>
    /// Gets the child node with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the child node to retrieve.</param>
    /// <returns>The child node with the specified ID.</returns>
    /// <exception cref="LsxMissingElementException">
    /// Thrown when a child node with the specified ID is not found.
    /// </exception>
    public LsxNode GetChild(string id) =>
        this.Children?.FirstOrDefault(x => x.Id == id) ?? throw new LsxMissingElementException(id);

    /// <summary>
    /// Gets the child node with the specified ID, or null if no such child node exists.
    /// </summary>
    /// <param name="id">The ID of the child node to retrieve.</param>
    /// <returns>The child node with the specified ID, or null if no such child node exists.</returns>
    public LsxNode? GetChildOrDefault(string id) =>
        this.Children?.FirstOrDefault(x => x.Id == id);
}
