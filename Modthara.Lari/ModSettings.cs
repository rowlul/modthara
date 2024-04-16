using System.IO.Abstractions;

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

    private List<Mod> _mods;
    public IReadOnlyList<Mod> Mods => _mods;

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
    public ModSettings(LariVersion version = default, IList<Mod>? mods = null) : this(new LsxDocument
    {
        Version = version,
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

    /// <summary>
    /// Creates a new instance from file.
    /// </summary>
    /// <param name="path">Path to the file.</param>
    /// <param name="fileStreamFactory">Wrapper for FileStream.</param>
    /// <returns>New instance of <see cref="ModSettings"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if serialized <see cref="LsxDocument"/> was null.</exception>
    public static ModSettings Read(string path, IFileStreamFactory fileStreamFactory)
    {
        using var file = fileStreamFactory.New(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return new ModSettings(LsxDocument.FromStream(file));
    }

    /// <summary>
    /// Creates a new instance from file.
    /// </summary>
    /// <param name="path">Path to the file.</param>
    /// <param name="fileStreamFactory">Wrapper for FileStream.</param>
    /// <returns>New instance of <see cref="ModSettings"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if serialized <see cref="LsxDocument"/> was null.</exception>
    public async static Task<ModSettings> ReadAsync(string path, IFileStreamFactory fileStreamFactory)
    {
        await using var file = fileStreamFactory.New(path, FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: 4096, useAsync: true);
        return new ModSettings(LsxDocument.FromStream(file));
    }

    /// <summary>
    /// Writes changes to the specified path, overwriting the file.
    /// </summary>
    /// <param name="path">Path to the file.</param>
    /// <param name="fileStreamFactory">Wrapper for FileStream.</param>
    public void Write(string path, IFileStreamFactory fileStreamFactory)
    {
        using var document = _document.ToStream();
        using var file = fileStreamFactory.New(path, FileMode.Create, FileAccess.Write, FileShare.None);
        document.CopyTo(file);
    }

    /// <summary>
    /// Writes changes to the specified path, overwriting the file.
    /// </summary>
    /// <param name="path">Path to the file.</param>
    /// <param name="fileStreamFactory">Wrapper for FileStream.</param>
    public async Task WriteAsync(string path, IFileStreamFactory fileStreamFactory)
    {
        await using var document = _document.ToStream();
        await using var file = fileStreamFactory.New(path, FileMode.Create, FileAccess.Write, FileShare.None,
            bufferSize: 4096, useAsync: true);
        await document.CopyToAsync(file);
    }

    /// <summary>
    /// Sanitizes the mod list in the correct order according to <c>ModOrder</c> node.
    /// Removes any duplicates.
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

        foreach (var mod in _mods)
        {
            mod.IsEnabled = _modsNode.Children.Any(modNode =>
                Guid.Parse(modNode.GetAttributeValue("UUID")) == mod.Uuid
                && _modOrderNode.Children.Any(modOrderNode =>
                    Guid.Parse(modOrderNode.GetAttributeValue("UUID")) == mod.Uuid
                )
            );
        }
    }

    /// <summary>
    /// Finds mod by UUID via traversing <c>ModOrder</c> and <c>Mods</c> node children and comparing indices.
    /// </summary>
    /// <param name="guid">UUID to search for.</param>
    /// <returns>Matched mod by UUID and its index in the order.</returns>
    public (Index?, Mod?) FindOrdered(Guid guid)
    {
        for (var i = 0; i < new[] { _modOrderNode.Children!.Count, _modsNode.Children!.Count, _mods.Count }.Min(); i++)
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

    /// <summary>
    /// Finds mod by UUID without order.
    /// </summary>
    /// <param name="guid">UUID to search for.</param>
    /// <returns>Matched mod by UUID and its index <c>Mods</c> node.</returns>
    public (Index?, Mod?) Find(Guid guid)
    {
        for (var i = 0; i < Math.Min(_modsNode.Children!.Count, _mods.Count); i++)
        {
            var modUuid = _modsNode.Children[i].GetUuid();
            if (modUuid == guid && _mods[i].Uuid == guid)
            {
                return (i, _modsNode.Children[i].ToMod());
            }
        }

        return (null, null);
    }

    /// <summary>
    /// Enables mod in the order.
    /// </summary>
    /// <param name="guid">UUID of the mod to enable.</param>
    public void Enable(Guid guid)
    {
        var (idx, mod) = Find(guid);

        if (idx == null || mod == null)
        {
            return;
        }

        if (mod.IsEnabled)
        {
            return;
        }

        _modOrderNode.Children!.Insert(idx.Value, mod.ToModule());
        mod.IsEnabled = true;
    }

    /// <summary>
    /// Disables mod in the order.
    /// </summary>
    /// <param name="guid">UUID of the mod to enable.</param>
    public void Disable(Guid guid)
    {
        var (idx, mod) = Find(guid);

        if (idx == null || mod == null)
        {
            return;
        }

        if (!mod.IsEnabled)
        {
            return;
        }

        _modOrderNode.Children!.RemoveAt(idx.Value);
        mod.IsEnabled = false;
    }

    /// <summary>
    /// Inserts mod at specified index.
    /// </summary>
    /// <param name="index">Index of the mod.</param>
    /// <param name="mod">Mod to be inserted.</param>
    public void Insert(int index, Mod mod)
    {
        _modOrderNode.Children!.Insert(index, mod.ToModule());
        _modsNode.Children!.Insert(index, mod.ToModuleShortDesc());
        _mods.Insert(index, mod);
    }

    /// <summary>
    /// Appends mod at the end of the list.
    /// </summary>
    /// <param name="mod">Mod to be appended.</param>
    public void Append(Mod mod)
    {
        _modOrderNode.Children!.Add(mod.ToModule());
        _modsNode.Children!.Add(mod.ToModuleShortDesc());
        _mods.Add(mod);
    }

    /// <summary>
    /// Removes mod from the list.
    /// </summary>
    /// <param name="mod">Mod to be removed.</param>
    public void Remove(Mod mod)
    {
        _modOrderNode.Children!.Remove(mod.ToModule());
        _modsNode.Children!.Remove(mod.ToModuleShortDesc());
        _mods.Remove(mod);
    }
}
