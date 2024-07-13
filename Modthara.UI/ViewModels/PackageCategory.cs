namespace Modthara.UI.ViewModels;

public enum PackageCategory
{
    All,
    Standalone,
    Overrides
}

public static class PackageCategoryExtensions
{
    public static Func<object, bool>? ToFilter(this PackageCategory category)
    {
        return category switch
        {
            PackageCategory.Standalone => x => (ModPackageViewModel)x is { HasModFiles: true },
            PackageCategory.Overrides => x => (ModPackageViewModel)x is { IsPureOverride: true },
            _ => null
        };
    }

    public static Func<object, bool>? ToFilter(this PackageCategory category, string nameSubstring)
    {
        return category switch
        {
            PackageCategory.Standalone => x =>
            {
                var m = (ModPackageViewModel)x;
                return m.HasModFiles &&
                       m.Name.Contains(nameSubstring, StringComparison.OrdinalIgnoreCase);
            },
            PackageCategory.Overrides => x =>
            {
                var m = (ModPackageViewModel)x;
                return m is { IsPureOverride: true } &&
                       m.Name.Contains(nameSubstring, StringComparison.OrdinalIgnoreCase);
            },
            _ => null
        };
    }
}
