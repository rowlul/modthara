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

    private List<LsxNode> ModOrderChildren
    {
        get => _document.GetRegion("ModuleSettings").GetNode("ModOrder").Children ??= [];
        set => _document.GetRegion("ModuleSettings").GetNode("ModOrder").Children = value;
    }

    private List<LsxNode> ModsChildren
    {
        get => _document.GetRegion("ModuleSettings").GetNode("Mods").Children ??= [];
        set => _document.GetRegion("ModuleSettings").GetNode("Mods").Children = value;
    }

    private List<Module> _mods;
    public IReadOnlyList<Module> Mods => _mods;

    /// <summary>
    /// Creates a new instance of <see cref="ModSettings"/> by parsing <paramref name="document"/>.
    /// </summary>
    /// <param name="document">
    /// Document containing <c>ModOrder</c> and <c>Mods</c> node under <c>ModuleSettings</c> region.
    /// </param>
    public ModSettings(LsxDocument document)
    {
        _document = document;
        _mods = ModsChildren.ToShortDescModules();
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

    public LsxDocument ToDocument() => _document;

    /// <summary>
    /// Sanitizes the mod list in the correct order according to <c>ModOrder</c> node.
    /// Removes any duplicates or single unpaired module nodes.
    /// </summary>
    public void Sanitize()
    {
        ModOrderChildren = ModOrderChildren.DistinctBy(n => n.GetUuid()).ToList();

        ModsChildren = ModsChildren.DistinctBy(n => n.GetUuid())
            .OrderBy(m =>
                ModOrderChildren.FindIndex(o => o.GetUuid() == m.GetUuid()))
            .ToList();

        _mods = _mods.DistinctBy(x => x.Uuid)
            .OrderBy(m => ModOrderChildren.FindIndex(o => o.GetUuid() == m.Uuid))
            .ToList();
    }

    /// <summary>
    /// Finds mod by UUID via traversing <c>ModOrder</c> and <c>Mods</c> node children and comparing indices.
    /// </summary>
    /// <param name="uuid">UUID to search for.</param>
    /// <returns>Matched mod by UUID and its index in the order.</returns>
    public (Index?, Module?) Find(LariUuid uuid)
    {
        for (var i = 0; i < new[] { ModOrderChildren.Count, ModsChildren.Count, _mods.Count }.Min(); i++)
        {
            var modOrderUuid = ModOrderChildren[i].GetUuid();
            var modUuid = ModsChildren[i].GetUuid();

            if (modOrderUuid.Value == uuid.Value && modUuid.Value == uuid.Value && _mods[i].Uuid.Value == uuid.Value)
            {
                return (i, new Module(ModsChildren[i]));
            }
        }

        return (null, null);
    }

    /// <summary>
    /// Inserts mod at specified index.
    /// </summary>
    /// <param name="index">Index of the mod.</param>
    /// <param name="mod">Mod to be inserted.</param>
    public void Insert(int index, Module mod)
    {
        ModOrderChildren.Insert(index, ((ModuleBase)mod).ToNode());
        ModsChildren.Insert(index, mod.ToNode());
        _mods.Insert(index, mod);
    }

    /// <summary>
    /// Appends mod at the end of the list.
    /// </summary>
    /// <param name="mod">Mod to be appended.</param>
    public void Append(Module mod)
    {
        ModOrderChildren.Add(((ModuleBase)mod).ToNode());
        ModsChildren.Add(mod.ToNode());
        _mods.Add(mod);
    }

    /// <summary>
    /// Removes mod from the list.
    /// </summary>
    /// <param name="mod">Mod to be removed.</param>
    public void Remove(Module mod)
    {
        ModOrderChildren.Remove(((ModuleBase)mod).ToNode());
        ModsChildren.Remove(mod.ToNode());
        _mods.Remove(mod);
    }
}
