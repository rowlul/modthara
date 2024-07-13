using Modthara.Lari;
using Modthara.Lari.Pak;

namespace Modthara.Manager;

public sealed class ModPackage
{
    public required string Path { get; set; }
    public required string Name { get; set; }
    public required Package Package { get; set; }
    public required ModMetadata? Metadata { get; set; }
    public required ModFlags Flags { get; set; }
    public required DateTime LastModified { get; set; }
}
