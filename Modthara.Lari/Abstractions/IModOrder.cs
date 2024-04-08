using System.IO.Abstractions;

namespace Modthara.Lari.Abstractions;

/// <summary>
/// Represents a generic mod order.
/// </summary>
public interface IModOrder
{
    IReadOnlyList<Mod> Mods { get; }
    void Write(string path, IFileStreamFactory fileStreamFactory);
    void Sanitize();
    (int?, Mod?) Find(Guid guid);
    void Insert(int index, Mod mod);
    void Append(Mod mod);
    void Remove(Mod mod);
}
