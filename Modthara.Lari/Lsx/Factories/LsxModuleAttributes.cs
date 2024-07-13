namespace Modthara.Lari.Lsx.Factories;

public record LsxModuleAttributes(
    LariUuid? Uuid = null,
    string? Folder = null,
    string? Md5 = null,
    string? Name = null,
    LariVersion? Version = null,
    string? Author = null,
    string? Description = null,
    IEnumerable<Module>? Modules = null);
