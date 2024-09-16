using System.Text.RegularExpressions;

using Modthara.Lari;
using Modthara.Lari.Pak;

using static Modthara.Manager.Constants;

namespace Modthara.Manager.Extensions;

public static partial class ModPackageExtensions
{
    public static ModPackage? GetMatchingModPackage(this List<ModPackage> packages, ModuleBase module)
    {
        foreach (var pkg in packages)
        {
            if (pkg.Metadata == null)
            {
                continue;
            }

            if (module.Uuid.Value == pkg.Metadata.Uuid.Value)
            {
                return pkg;
            }
        }

        return null;
    }

    public static PackagedFile? GetMetaLsxFile(this Package pak)
    {
        foreach (var file in pak.Files)
        {
            var match = MetaRegex().Match(file.Name);
            if (match.Success && CommonAlteredGameFiles.All(gameFile => match.Groups[1].Value != gameFile))
            {
                return file;
            }
        }

        return null;
    }

    public static bool IsGameFile(this ModuleBase mod) => IgnoredModUuids.Any(x => mod.Uuid == x);

    public static ModFlags GetScriptExtenderFlag(this Package pak) =>
        pak.Files.Select(file => ScriptExtenderConfigRegex().Match(file.Name)).Any(match => match.Success)
            ? ModFlags.ScriptExtender
            : ModFlags.None;

    public static ModFlags GetForceRecompileFlag(this Package pak) =>
        pak.Files.Select(file => ForceRecompileRegex().Match(file.Name)).Any(match => match.Success)
            ? ModFlags.ForceRecompile
            : ModFlags.None;

    public static ModFlags GetGameOverrideFlag(this Package pak) =>
        pak.Files.Any(file => file.Name.StartsWith("Video/") ||
                              CommonAlteredGameFiles.Any(f =>
                                  file.Name.StartsWith("Public/" + f) ||
                                  file.Name.StartsWith("Generated/Public/" + f) ||
                                  file.Name.StartsWith($"Mods/{f}/Public") ||
                                  file.Name.StartsWith($"Mods/{f}/Generated/Public")))
            ? ModFlags.GameOverride
            : ModFlags.None;

    public static ModFlags GetModAdditionFlag(this Package pak)
    {
        foreach (var file in pak.Files)
        {
            var hasScripts = file.Name.StartsWith("Scripts/");
            if (hasScripts)
            {
                return ModFlags.ModAddition;
            }

            var modsFolderMatch = ModsFolderRegex().Match(file.Name);
            if (modsFolderMatch.Success && CommonAlteredGameFiles.All(f => f != modsFolderMatch.Groups[1].Value) &&
                file.Name != $"Mods/{modsFolderMatch.Groups[1].Value}/meta.lsx" &&
                file.Name != $"Mods/{modsFolderMatch.Groups[1].Value}/Story/RawFiles/Goals/ForceRecompile.txt")
            {
                return ModFlags.ModAddition;
            }

            var publicFolderMatch = PublicFolderRegex().Match(file.Name);
            if (publicFolderMatch.Success && CommonAlteredGameFiles.All(f => f != publicFolderMatch.Groups[1].Value))
            {
                return ModFlags.ModAddition;
            }
        }

        return ModFlags.None;
    }

    [GeneratedRegex("^Mods/([^/]+)/Story/RawFiles/Goals/ForceRecompile\\.txt$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex ForceRecompileRegex();

    [GeneratedRegex("^Mods/([^/]+)/meta\\.lsx$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex MetaRegex();

    [GeneratedRegex("^Mods/([^/]+)/.*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex ModsFolderRegex();

    [GeneratedRegex("^Public/([^/]+)/.*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex PublicFolderRegex();

    [GeneratedRegex("^Mods/([^/]+)/ScriptExtender/Config\\.json$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex ScriptExtenderConfigRegex();
}
