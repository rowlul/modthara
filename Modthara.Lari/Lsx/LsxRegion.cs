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

    /// <summary>
    /// Gets the node with the specified ID from the root node's children.
    /// </summary>
    /// <param name="id">The ID of the node to retrieve.</param>
    /// <returns>The node with the specified ID.</returns>
    /// <exception cref="LsxMissingElementException">
    /// Thrown when a node with the specified ID is not found in the root node's children.
    /// </exception>
    public LsxNode GetNode(string id)
    {
        if (this.RootNode is { Id: "root", Children: not null })
        {
            return this.RootNode.Children.FirstOrDefault(x => x.Id == id)
                   ?? throw new LsxMissingElementException(id);
        }

        throw new LsxMissingElementException(id);
    }

    /// <summary>
    /// Gets the node with the specified ID from the root node's children, or null if no such node exists.
    /// </summary>
    /// <param name="id">The ID of the node to retrieve.</param>
    /// <returns>The node with the specified ID, or null if no such node exists.</returns>
    public LsxNode? GetNodeOrDefault(string id) =>
        this.RootNode is { Id: "root", Children: not null }
            ? this.RootNode.Children.FirstOrDefault(x => x.Id == id)
            : null;
}
