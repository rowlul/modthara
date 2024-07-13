using Modthara.Lari;

namespace Modthara.Manager.Extensions;

public static class ModPackageExtensions
{
    public static ModPackage? FindMatchingMod(this IEnumerable<ModPackage> collection, ModuleBase module)
    {
        foreach (var pkg in collection)
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
