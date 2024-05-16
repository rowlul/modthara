using Modthara.Lari.Lsx;

namespace Modthara.Lari;

/// <summary>
/// Represents <c>meta.lsx</c> of a mod.
/// </summary>
public class ModMetadata
{
    public required string Name { get; set; }
    public string? Author { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public required string FolderName { get; set; }
    public string Md5 { get; set; } = string.Empty;
    public LariUuid Uuid { get; set; } = LariUuid.NewGuid();
    public LariVersion Version { get; set; } = DefaultLariVersion;
    public IList<ModMetadata>? Dependencies { get; set; }

    /// <summary>
    /// Creates a new instance from <see cref="LsxDocument"/>.
    /// </summary>
    /// <param name="document">Document containing the <c>ModuleInfo</c> node under <c>Config</c> region.</param>
    /// <returns>Instance of <see cref="ModMetadata"/>.</returns>
    public static ModMetadata FromLsx(LsxDocument document)
    {
        var mod = document.GetNode("Config", "ModuleInfo").ToModMetadata();

        var dependencies = document.GetNodeOrDefault("Config", "Dependencies");
        if (dependencies != null)
        {
            mod.Dependencies = dependencies.GetModules();
        }

        return mod;
    }

    /// <summary>
    /// Maps instance to <c>Module</c> node.
    /// </summary>
    /// <returns>Instance of <c>Module</c> node.</returns>
    public LsxNode ToModule() =>
        new()
        {
            Id = "Module",
            Attributes =
            [
                new LsxAttribute { Id = "UUID", Type = "FixedString", Value = this.Uuid.Value },
            ]
        };

    /// <summary>
    /// Maps instance to <c>ModuleShortDesc</c> node.
    /// </summary>
    /// <returns>Instance of <c>ModuleShortDesc</c> node.</returns>
    public LsxNode ToModuleShortDesc() =>
        new()
        {
            Id = "ModuleShortDesc",
            Attributes =
            [
                new LsxAttribute { Id = "Folder", Type = "LSString", Value = this.FolderName },
                new LsxAttribute { Id = "MD5", Type = "LSString", Value = this.Md5 },
                new LsxAttribute { Id = "Name", Type = "LSString", Value = this.Name },
                new LsxAttribute { Id = "UUID", Type = "FixedString", Value = this.Uuid.Value },
                new LsxAttribute
                {
                    Id = "Version64", Type = "int64", Value = LariVersion.ToVersion64(this.Version).ToString()
                }
            ]
        };

    /// <summary>
    /// Maps instance to <c>ModuleInfo</c> node.
    /// </summary>
    /// <returns>Instance of <c>ModuleInfo</c> node.</returns>
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
                new LsxAttribute { Id = "UUID", Type = "FixedString", Value = this.Uuid.Value },
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

    private static readonly LariVersion DefaultLariVersion = 36028797018963968UL;
}
