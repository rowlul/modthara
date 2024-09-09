using Modthara.Lari;

namespace Modthara.Manager.Extensions;

public static class ModPackageExtensions
{
    public static ModPackage? FindMatchingMod(this List<ModPackage> packages, ModuleBase module)
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

    public static bool IsGameFile(this ModuleBase mod) => Constants.IgnoredModUuids.Any(x => mod.Uuid == x);
}
