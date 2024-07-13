namespace Modthara.Lari.Lsx.Factories;

public static class LsxDocumentFactory
{
    public static LsxDocument CreateMeta(LsxModuleAttributes attributes)
    {
        var region = LsxRegionFactory.CreateConfig(attributes);
        var document = new LsxDocument { Version = new LariVersion(4, 0, 6, 68), Regions = [region] };
        return document;
    }

    public static LsxDocument CreateModSettings(LsxModuleAttributes attributes)
    {
        var region = LsxRegionFactory.CreateModuleSettings(attributes);
        var document = new LsxDocument { Version = new LariVersion(4, 6, 0, 900), Regions = [region] };
        return document;
    }
}
