using Modthara.Lari.Abstractions;
using Modthara.Lari.Lsx;

namespace Modthara.Lari;

/// <summary>
/// Represents the means to handling <c>modsettings.lsx</c>.
/// </summary>
public class ModSettings : IModOrder
{
    private readonly LsxDocument _document;
    private readonly LsxNode _modOrderNode;
    private readonly LsxNode _modsNode;

    private List<Mod> _mods;
    public IReadOnlyList<Mod> Mods => _mods;

    public ModSettings(LsxDocument document)
    {
        _document = document;

        _modOrderNode = document.FindNodeInRoot("ModuleSettings", "ModOrder");
        _modOrderNode.Children ??= [];

        _modsNode = document.FindNodeInRoot("ModuleSettings", "Mods");
        _modOrderNode.Children ??= [];

        _mods = _modsNode.GetModules();
    }

    public static ModSettings Read(string path)
    {
        using var file = File.OpenRead(path);
        return new ModSettings(
            LsxDocument.FromStream(file) ??
            throw new InvalidOperationException($"Document '{path}' was null."));
    }

    public void Write(string path)
    {
        using var document = _document.ToStream();
        using var file = File.Create(path);
        document.CopyTo(file);
    }

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
    /// <param name="guid">UUID to search for.</param>
    /// <returns>Matched mod by UUID and its index in the order.</returns>
    public (int?, Mod?) Find(Guid guid)
    {
        for (int i = 0; i < new[] { _modOrderNode.Children!.Count, _modsNode.Children!.Count, _mods.Count }.Min(); i++)
        {
            var modOrderUuid = _modOrderNode.Children[i].GetUuid();
            var modUuid = _modsNode.Children[i].GetUuid();

            if (modOrderUuid == guid && modUuid == guid && _mods[i].Uuid == guid)
            {
                return (i, _modsNode.Children[i].ToMod());
            }
        }

        return (null, null);
    }

    public void Insert(int index, Mod mod)
    {
        _modOrderNode.Children!.Insert(index, mod.ToModule());
        _modsNode.Children!.Insert(index, mod.ToModuleShortDesc());
        _mods.Insert(index, mod);
    }

    public void Append(Mod mod)
    {
        _modOrderNode.Children!.Add(mod.ToModule());
        _modsNode.Children!.Add(mod.ToModuleShortDesc());
        _mods.Add(mod);
    }

    public void Remove(Mod mod)
    {
        _modOrderNode.Children!.Remove(mod.ToModule());
        _modsNode.Children!.Remove(mod.ToModuleShortDesc());
        _mods.Remove(mod);
    }
}
