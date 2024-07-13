using Modthara.Lari.Extensions;
using Modthara.Lari.Lsx;
using Modthara.Lari.Lsx.Factories;

namespace Modthara.Lari;

/// <summary>
/// Represents <c>ModuleShortDesc</c> node.
/// </summary>
public class Module : ModuleBase
{
    public string FolderName { get; set; }
    public string Md5 { get; set; }
    public string Name { get; set; }
    public LariVersion Version { get; set; }

    public Module(string folderName, string md5, string name, LariUuid uuid, LariVersion version) : base(uuid)
    {
        FolderName = folderName;
        Md5 = md5;
        Name = name;
        Uuid = uuid;
        Version = version;
    }

    public Module(LsxNode node) : base(node.GetUuid())
    {
        FolderName = node.GetAttribute("Folder").Value;
        Md5 = node.GetAttribute("MD5").Value;
        Name = node.GetAttribute("Name").Value;
        Version = node.GetVersion();
    }

    public new LsxNode ToNode() =>
        LsxNodeFactory.CreateModuleShortDesc(new LsxModuleAttributes(Uuid, FolderName, Md5, Name, Version));
}
