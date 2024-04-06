namespace Modthara.Lari;

public readonly struct ModVersion
{
    public uint Major { get; init; }
    public uint Minor { get; init; }
    public uint Revision { get; init; }
    public uint Build { get; init; }

    public static ModVersion FromUint64(ulong version) => new()
    {
        Major = (uint)((version >> 55) & 0x7f),
        Minor = (uint)((version >> 47) & 0xff),
        Revision = (uint)((version >> 31) & 0xffff),
        Build = (uint)(version & 0x7fffffff),
    };

    public static ModVersion FromUint32(uint version) => new ModVersion
    {
        Major = (version >> 28) & 0x0f,
        Minor = (version >> 24) & 0x0f,
        Revision = (version >> 16) & 0xff,
        Build = version & 0xffff,
    };

    public static ulong ToVersion64(ModVersion version) =>
        (ulong)(((long)version.Major & 0x7f) << 55 |
                ((long)version.Minor & 0xff) << 47 |
                ((long)version.Revision & 0xffff) << 31 |
                ((long)version.Build & 0x7fffffff) << 0);

    public static uint ToVersion32(ModVersion version) =>
        (version.Major & 0x0f) << 28 |
        (version.Minor & 0x0f) << 24 |
        (version.Revision & 0xff) << 16 |
        (version.Build & 0xffff) << 0;

    public override string ToString()
    {
        return $"{Major}.{Minor}.{Revision}.{Build}";
    }

    public static implicit operator ModVersion(ulong version) => FromUint64(version);
    public static implicit operator ModVersion(uint version) => FromUint32(version);
    public static implicit operator ulong(ModVersion version) => ToVersion64(version);
    public static implicit operator uint(ModVersion version) => ToVersion32(version);
}
