using Modthara.Lari;

namespace Modthara.App.Models;

public class ModtharaMod : ModMetadata
{
    public required bool IsEnabled { get; set; }
    public required string FilePath { get; init; }
    public required ModFlags Flags { get; init; }

    public string StringifiedVersion => Version.ToString();
}
