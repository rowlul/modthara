namespace Modthara.Lari.Lsx.Factories;

public static class LsxRegionFactory
{
    public static LsxRegion CreateConfig(LsxModuleAttributes attributes)
    {
        var moduleInfo = LsxNodeFactory.CreateModuleInfo(attributes);
        var root = new LsxNode { Id = "root", Children = [moduleInfo] };

        if (attributes.Modules != null)
        {
            var dependencies = LsxNodeFactory.CreateDependencies(attributes);
            root.Children.Add(dependencies);
        }

        var region = new LsxRegion { Id = "Config", RootNode = root };
        return region;
    }

    public static LsxRegion CreateModuleSettings(LsxModuleAttributes attributes)
    {
        var modOrder = LsxNodeFactory.CreateModOrder(attributes);
        var mods = LsxNodeFactory.CreateMods(attributes);
        var root = new LsxNode { Id = "root", Children = [modOrder, mods] };
        var region = new LsxRegion { Id = "ModuleSettings", RootNode = root };
        return region;
    }
}
