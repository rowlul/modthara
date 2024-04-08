using Modthara.Lari.Lsx;

namespace Modthara.Lari;

public class Mod
{
    public required string Name { get; set; }
    public string? Author { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public required string FolderName { get; set; }
    public string Md5 { get; set; } = string.Empty;
    public Guid Uuid { get; set; } = Guid.NewGuid();
    public LariVersion Version { get; set; } = 36028797018963968UL;
    public IList<Mod> Dependencies { get; set; } = [];

    public static Mod FromLsx(LsxDocument document)
    {
        var mod = document.FindNodeInRoot("Config", "ModuleInfo").ToMod();
        mod.Dependencies = document.FindNodeInRoot("Config", "Dependencies").GetModules();

        return mod;
    }

    public LsxNode ToModule() =>
        new()
        {
            Id = "Module",
            Attributes =
            [
                new LsxAttribute { Id = "UUID", Type = "FixedString", Value = this.Uuid.ToString() },
            ]
        };

    public LsxNode ToModuleShortDesc() =>
        new()
        {
            Id = "ModuleShortDesc",
            Attributes =
            [
                new LsxAttribute { Id = "Folder", Type = "LSString", Value = this.FolderName },
                new LsxAttribute { Id = "MD5", Type = "LSString", Value = this.Md5 },
                new LsxAttribute { Id = "Name", Type = "LSString", Value = this.Name },
                new LsxAttribute { Id = "UUID", Type = "FixedString", Value = this.Uuid.ToString() },
                new LsxAttribute
                {
                    Id = "Version64", Type = "int64", Value = LariVersion.ToVersion64(this.Version).ToString()
                }
            ]
        };

    public LsxNode ToModuleInfo() =>
        new()
        {
            Id = "ModuleInfo",
            Attributes =
            [
                new LsxAttribute { Id = "Author", Type = "LSWString", Value = this.Author ?? string.Empty },
                new LsxAttribute { Id = "CharacterCreationLevelName", Type = "FixedString", Value = string.Empty },
                new LsxAttribute { Id = "Description", Type = "LSWString", Value = this.Description ?? string.Empty },
                new LsxAttribute { Id = "Folder", Type = "LSWString", Value = this.FolderName },
                new LsxAttribute { Id = "GMTemplate", Type = "FixedString", Value = string.Empty },
                new LsxAttribute { Id = "LobbyLevelName", Type = "FixedString", Value = string.Empty },
                new LsxAttribute { Id = "MD5", Type = "LSString", Value = this.Md5 },
                new LsxAttribute { Id = "MainMenuBackgroundVideo", Type = "FixedString", Value = string.Empty },
                new LsxAttribute { Id = "MenuLevelName", Type = "FixedString", Value = string.Empty },
                new LsxAttribute { Id = "Name", Type = "FixedString", Value = this.Name },
                new LsxAttribute { Id = "NumPlayers", Type = "uint8", Value = "4" },
                new LsxAttribute { Id = "PhotoBooth", Type = "FixedString", Value = string.Empty },
                new LsxAttribute { Id = "StartupLevelName", Type = "FixedString", Value = string.Empty },
                new LsxAttribute { Id = "Tags", Type = "LSWString", Value = string.Empty },
                new LsxAttribute { Id = "Type", Type = "FixedString", Value = "Add-on" },
                new LsxAttribute { Id = "UUID", Type = "FixedString", Value = this.Uuid.ToString() },
                new LsxAttribute
                {
                    Id = "Version64", Type = "int64", Value = LariVersion.ToVersion64(this.Version).ToString()
                }
            ],
            Children =
            [
                new LsxNode
                {
                    Id = "PublishVersion",
                    Attributes =
                    [
                        new LsxAttribute
                        {
                            Id = "Version64",
                            Type = "int64",
                            Value = LariVersion.ToVersion64(this.Version).ToString()
                        }
                    ]
                },
                new LsxNode { Id = "Scripts" },
                new LsxNode
                {
                    Id = "TargetModes",
                    Children =
                    [
                        new LsxNode
                        {
                            Id = "Target",
                            Attributes =
                            [
                                new LsxAttribute { Id = "Object", Type = "FixedString", Value = "Story" }
                            ]
                        }
                    ]
                }
            ]
        };
}
