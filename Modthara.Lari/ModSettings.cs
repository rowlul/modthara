using Modthara.Lari.Extensions;
using Modthara.Lari.Lsx;
using Modthara.Lari.Lsx.Factories;

namespace Modthara.Lari;

using Index = int;

/// <summary>
/// Represents <c>ModuleSettings</c> region.
/// </summary>
public class ModSettings
{
    private readonly LsxDocument _document;

    private List<LsxNode> ModsChildren
    {
        get => _document.GetRegion("ModuleSettings").GetNode("Mods").Children ??= [];
        set => _document.GetRegion("ModuleSettings").GetNode("Mods").Children = value;
    }

    private List<Module> _modules;

    /// <summary>
    /// Gets the list of modules.
    /// </summary>
    /// <value>
    /// An <see cref="IReadOnlyList{Module}"/> representing the list of modules.
    /// </value>
    public IReadOnlyList<Module> Modules => _modules;

    /// <summary>
    /// Creates a new instance of <see cref="ModSettings"/> by parsing <paramref name="document"/>.
    /// </summary>
    /// <param name="document">
    /// Document containing <c>ModOrder</c> and <c>Mods</c> node under <c>ModuleSettings</c> region.
    /// </param>
    public ModSettings(LsxDocument document)
    {
        _document = document;
        _modules = ModsChildren.ToShortDescModules();
    }

    /// <summary>
    /// Creates a new instance of <see cref="ModSettings"/> with empty mod order.
    /// </summary>
    /// <param name="mods">
    /// Mods to be included.
    /// </param>
    public ModSettings(IEnumerable<Module>? mods = null) : this(
        LsxDocumentFactory.CreateModSettings(new LsxModuleAttributes(Modules: mods ?? [])))
    {
    }

    /// <summary>
    /// Converts the current instance of <see cref="ModSettings"/> to an <see cref="LsxDocument"/>.
    /// </summary>
    /// <returns>
    /// The <see cref="LsxDocument"/> representation of the current <see cref="ModSettings"/>.
    /// </returns>
    public LsxDocument ToDocument() => _document;

    /// <summary>
    /// Sanitizes the mod list in the correct order according to <c>ModOrder</c> node.
    /// Removes any duplicates or single unpaired module nodes.
    /// </summary>
    public void Sanitize()
    {
        ModsChildren = ModsChildren.DistinctBy(n => n.GetUuid()).ToList();
        _modules = _modules.DistinctBy(x => x.Uuid).ToList();
    }

    /// <summary>
    /// Inserts mod at specified index.
    /// </summary>
    /// <param name="index">
    /// Index of the mod.
    /// </param>
    /// <param name="mod">
    /// Mod to be inserted.
    /// </param>
    public void Insert(int index, Module mod)
    {
        ModsChildren.Insert(index, mod.ToNode());
        _modules.Insert(index, mod);
    }

    /// <summary>
    /// Appends mod at the end of the list.
    /// </summary>
    /// <param name="mod">
    /// Mod to be appended.
    /// </param>
    public void Append(Module mod)
    {
        ModsChildren.Add(mod.ToNode());
        _modules.Add(mod);
    }

    /// <summary>
    /// Removes mod from the list.
    /// </summary>
    /// <param name="mod">
    /// Mod to be removed.
    /// </param>
    /// <returns>
    /// <c>true</c> if the mod was successfully removed; otherwise, <c>false</c> if the mod was not found.
    /// </returns>
    public bool Remove(Module mod)
    {
        try
        {
            ModsChildren.RemoveAt(ModsChildren.FindIndex(m => m.GetUuid() == mod.Uuid));
            _modules.RemoveAt(_modules.FindIndex(m => m.Uuid == mod.Uuid));

            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
    }
}
