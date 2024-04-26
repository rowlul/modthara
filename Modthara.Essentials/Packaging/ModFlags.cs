namespace Modthara.Essentials.Packaging;

[Flags]
public enum ModFlags
{
    // @formatter:off
    None                   = 0b0,
    HasModFiles            = 0b_0001,
    AltersGameFiles        = 0b_0010,
    HasForceRecompile      = 0b_0100,
    RequiresScriptExtender = 0b_1000,
    // @formatter:on
}
