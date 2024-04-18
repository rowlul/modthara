namespace Modthara.Essentials.Packaging;

[Flags]
public enum ModFlags
{
    None,
    HasOwnContent,
    HasGameOverrides,
    HasModFixer,
    RequiresScriptExtender
}
