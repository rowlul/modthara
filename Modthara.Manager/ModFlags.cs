namespace Modthara.Manager;

[Flags]
public enum ModFlags
{
    None = 0,
    HasModFiles = 1,
    AltersGameFiles = 1 << 1,
    HasForceRecompile = 1 << 2,
    RequiresScriptExtender = 1 << 3,
    Enabled = 1 << 4
}
