namespace Modthara.Lari.Lsx.Factories;

public static class LsxNodeFactory
{
    public static LsxNode CreateModule(LsxModuleAttributes attributes)
    {
        if (!attributes.Uuid.HasValue)
        {
            throw new ArgumentException("UUID must not be null.", nameof(attributes));
        }

        var node = new LsxNode
        {
            Id = "Module",
            Attributes =
                [new LsxAttribute { Id = "UUID", Type = "FixedString", Value = attributes.Uuid.Value.Value }]
        };

        return node;
    }

    public static LsxNode CreateModuleShortDesc(LsxModuleAttributes attributes)
    {
        var (uuid, folder, md5, name, version, _, _, _) = attributes;

        if (!uuid.HasValue)
        {
            throw new ArgumentException("UUID must not be null.", nameof(attributes));
        }

        if (!version.HasValue)
        {
            throw new ArgumentException("Version must not be null.", nameof(attributes));
        }

        if (md5 == null)
        {
            throw new ArgumentException("MD5 must not be null", nameof(attributes));
        }

        ArgumentException.ThrowIfNullOrEmpty(folder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var node = new LsxNode
        {
            Id = "ModuleShortDesc",
            Attributes =
            [
                new LsxAttribute { Id = "Folder", Type = "LSString", Value = folder },
                new LsxAttribute { Id = "MD5", Type = "LSString", Value = md5 },
                new LsxAttribute { Id = "Name", Type = "LSString", Value = name },
                new LsxAttribute { Id = "UUID", Type = "FixedString", Value = uuid.Value.Value },
                new LsxAttribute { Id = "Version64", Type = "int64", Value = version.Value.ToUInt64().ToString() }
            ]
        };

        return node;
    }

    public static LsxNode CreateModOrder(LsxModuleAttributes attributes)
    {
        if (attributes.Modules == null)
        {
            throw new ArgumentException("Modules must not be null.", nameof(attributes));
        }

        var children = attributes.Modules.Select(x => ((ModuleBase)x).ToNode()).ToList();
        var node = new LsxNode { Id = "ModOrder", Children = children };
        return node;
    }

    public static LsxNode CreateMods(LsxModuleAttributes attributes)
    {
        if (attributes.Modules == null)
        {
            throw new ArgumentException("Modules must not be null.", nameof(attributes));
        }

        var children = attributes.Modules.Select(x => x.ToNode()).ToList();
        var node = new LsxNode { Id = "Mods", Children = children };
        return node;
    }

    public static LsxNode CreateDependencies(LsxModuleAttributes attributes)
    {
        if (attributes.Modules == null)
        {
            throw new ArgumentException("Modules must not be null.", nameof(attributes));
        }

        var children = attributes.Modules.Select(x => x.ToNode()).ToList();
        var node = new LsxNode { Id = "Dependencies", Children = children };
        return node;
    }

    public static LsxNode CreateModuleInfo(LsxModuleAttributes attributes)
    {
        var (uuid, folder, md5, name, version, author, description, _) = attributes;

        if (!uuid.HasValue)
        {
            throw new ArgumentException("UUID must not be null.", nameof(attributes));
        }

        if (!version.HasValue)
        {
            throw new ArgumentException("Version must not be null.", nameof(attributes));
        }

        if (md5 == null)
        {
            throw new ArgumentException("MD5 must not be null", nameof(attributes));
        }

        ArgumentException.ThrowIfNullOrEmpty(folder);
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(author);
        ArgumentException.ThrowIfNullOrEmpty(description);

        var node = new LsxNode
        {
            Id = "ModuleInfo",
            Attributes =
            [
                new LsxAttribute { Id = "Author", Type = "LSWString", Value = author },
                new LsxAttribute { Id = "CharacterCreationLevelName", Type = "FixedString", Value = string.Empty },
                new LsxAttribute { Id = "Description", Type = "LSWString", Value = description },
                new LsxAttribute { Id = "Folder", Type = "LSWString", Value = folder },
                new LsxAttribute { Id = "GMTemplate", Type = "FixedString", Value = string.Empty },
                new LsxAttribute { Id = "LobbyLevelName", Type = "FixedString", Value = string.Empty },
                new LsxAttribute { Id = "MD5", Type = "LSString", Value = md5 },
                new LsxAttribute { Id = "MainMenuBackgroundVideo", Type = "FixedString", Value = string.Empty },
                new LsxAttribute { Id = "MenuLevelName", Type = "FixedString", Value = string.Empty },
                new LsxAttribute { Id = "Name", Type = "FixedString", Value = name },
                new LsxAttribute { Id = "NumPlayers", Type = "uint8", Value = "4" },
                new LsxAttribute { Id = "PhotoBooth", Type = "FixedString", Value = string.Empty },
                new LsxAttribute { Id = "StartupLevelName", Type = "FixedString", Value = string.Empty },
                new LsxAttribute { Id = "Tags", Type = "LSWString", Value = string.Empty },
                new LsxAttribute { Id = "Type", Type = "FixedString", Value = "Add-on" },
                new LsxAttribute { Id = "UUID", Type = "FixedString", Value = uuid.Value.Value },
                new LsxAttribute { Id = "Version64", Type = "int64", Value = version.Value.ToUInt64().ToString() }
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
                            Id = "Version64", Type = "int64", Value = version.Value.ToUInt64().ToString()
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


        return node;
    }
}
