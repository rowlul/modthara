namespace Modthara.App.ViewModels;

public enum PackageCategory
{
    All,
    Standalone,
    Overrides
}

public static class PackageCategoryExtensions
{
    public static Func<ModPackageViewModel, bool>? ToPredicate(this PackageCategory category)
    {
        return category switch
        {
            PackageCategory.Standalone => x => x.HasModFiles,
            PackageCategory.Overrides => x => x.HasOverrides && !x.HasModFiles,
            _ => null
        };
    }
}
