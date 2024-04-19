using Humanizer;

using Modthara.Lari;
using Modthara.Lari.Pak;

namespace Modthara.Essentials.Packaging;

public class ModPackage : ModMetadata
{
    public bool IsEnabled { get; set; }
    public required Package Package { get; init; }
    public required ModFlags Flags { get; init; }
    public DateTime LastModified { get; set; } = DateTime.Now;

    public string StringifiedVersion => Version.ToString();
    public string HumanizedLastModified => LastModified.Humanize();
}
