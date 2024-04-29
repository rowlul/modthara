﻿using Modthara.Lari;
using Modthara.Lari.Pak;

namespace Modthara.Essentials.Packaging;

public class ModPackage : ModMetadata
{
    public required string Path { get; set; }
    public required Package Package { get; init; }
    public required ModFlags Flags { get; set; }
    public DateTime LastModified { get; set; } = DateTime.Now;
    public PackagedFile? ScriptExtenderConfig { get; set; }
}
