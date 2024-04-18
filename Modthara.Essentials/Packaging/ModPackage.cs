using Modthara.Lari;
using Modthara.Lari.Pak;

namespace Modthara.Essentials.Packaging;

public class ModPackage : ModMetadata
{
    public bool IsEnabled { get; set; }
    public required Package Package { get; init; }
    public required ModFlags Flags { get; init; }
    public DateOnly LastModified { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public string StringifiedVersion => Version.ToString();
}
