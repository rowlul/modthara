using Modthara.Lari.Lsx;

namespace Modthara.Lari;

using Index = int;

/// <summary>
/// Represents the means to handling <c>modsettings.lsx</c>.
/// </summary>
public class ModSettings
{
    private readonly LsxDocument _document;
    private readonly LsxNode _modOrderNode;
    private readonly LsxNode _modsNode;

    private List<ModMetadata> _mods;
    public IReadOnlyList<ModMetadata> Mods => _mods;

    /// <summary>
    /// Creates a new instance of <see cref="ModSettings"/> by parsing <paramref name="document"/>.
    /// </summary>
    /// <param name="document">
    /// Document containing <c>ModOrder</c> and <c>Mods</c> node under <c>ModuleSettings</c> region.
    /// </param>
    public ModSettings(LsxDocument document)
    {
        _document = document;

        _modOrderNode = document.FindNodeInRoot("ModuleSettings", "ModOrder");
        _modOrderNode.Children ??= [];

        _modsNode = document.FindNodeInRoot("ModuleSettings", "Mods");
        _modOrderNode.Children ??= [];

        _mods = _modsNode.GetModules();
    }

    /// <summary>
    /// Creates a new instance of <see cref="ModSettings"/> with empty mod order.
    /// </summary>
    /// <param name="version">
    /// Version of the underlying <see cref="LsxDocument"/>.
    /// </param>
    /// <param name="mods">
    /// Mods to be included.
    /// </param>
    public ModSettings(LariVersion? version = null, IList<ModMetadata>? mods = null) : this(new LsxDocument
    {
        Version = DefaultLariVersion,
        Regions =
        [
            new LsxRegion
            {
                Id = "ModuleSettings",
                RootNode = new LsxNode
                {
                    Id = "root",
                    Children =
                    [
                        new LsxNode { Id = "ModOrder", Children = [] },
                        new LsxNode { Id = "Mods", Children = [] },
                    ]
                }
            }
        ]
    })
    {
        if (mods != null)
        {
            foreach (var mod in mods)
            {
                this.Append(mod);
            }
        }
    }

    /// <inheritdoc cref="LsxDocument.ToStream"/>
    public Stream ToStream() => _document.ToStream();

    /// <summary>
    /// Sanitizes the mod list in the correct order according to <c>ModOrder</c> node.
    /// Removes any duplicates or single unpaired module nodes.
    /// </summary>
    public void Sanitize()
    {
        _modOrderNode.Children = _modOrderNode.Children!.DistinctBy(n => n.GetUuid()).ToList();
        _modsNode.Children = _modsNode.Children!.DistinctBy(n => n.GetUuid())
            .OrderBy(m =>
                _modOrderNode.Children.FindIndex(o => o.GetUuid() == m.GetUuid()))
            .ToList();
        _mods = _mods.DistinctBy(x => x.Uuid)
            .OrderBy(m => _modOrderNode.Children.FindIndex(o => o.GetUuid() == m.Uuid))
            .ToList();
    }

    /// <summary>
    /// Finds mod by UUID via traversing <c>ModOrder</c> and <c>Mods</c> node children and comparing indices.
    /// </summary>
    /// <param name="uuid">UUID to search for.</param>
    /// <returns>Matched mod by UUID and its index in the order.</returns>
    public (Index?, ModMetadata?) Find(LariUuid uuid)
    {
        for (var i = 0; i < new[] { _modOrderNode.Children!.Count, _modsNode.Children!.Count, _mods.Count }.Min(); i++)
        {
            var modOrderUuid = _modOrderNode.Children[i].GetUuid();
            var modUuid = _modsNode.Children[i].GetUuid();

            if (modOrderUuid == uuid && modUuid == uuid && _mods[i].Uuid == uuid)
            {
                return (i, _modsNode.Children[i].ToModMetadata());
            }
        }

        return (null, null);
    }

    /// <summary>
    /// Inserts mod at specified index.
    /// </summary>
    /// <param name="index">Index of the mod.</param>
    /// <param name="modMetadata">Mod to be inserted.</param>
    public void Insert(int index, ModMetadata modMetadata)
    {
        _modOrderNode.Children!.Insert(index, modMetadata.ToModule());
        _modsNode.Children!.Insert(index, modMetadata.ToModuleShortDesc());
        _mods.Insert(index, modMetadata);
    }

    /// <summary>
    /// Appends mod at the end of the list.
    /// </summary>
    /// <param name="modMetadata">Mod to be appended.</param>
    public void Append(ModMetadata modMetadata)
    {
        _modOrderNode.Children!.Add(modMetadata.ToModule());
        _modsNode.Children!.Add(modMetadata.ToModuleShortDesc());
        _mods.Add(modMetadata);
    }

    /// <summary>
    /// Removes mod from the list.
    /// </summary>
    /// <param name="modMetadata">Mod to be removed.</param>
    public void Remove(ModMetadata modMetadata)
    {
        _modOrderNode.Children!.Remove(modMetadata.ToModule());
        _modsNode.Children!.Remove(modMetadata.ToModuleShortDesc());
        _mods.Remove(modMetadata);
    }

    private static readonly LariVersion DefaultLariVersion = 144959613005988740;
}
