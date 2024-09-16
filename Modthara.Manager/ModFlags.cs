namespace Modthara.Manager;

[Flags]
public enum ModFlags
{
    None = 0,
    ModAddition = 1,
    GameOverride = 1 << 1,
    ForceRecompile = 1 << 2,
    ScriptExtender = 1 << 3
}
