using System.Xml.Serialization;

namespace Modthara.Lari;

/// <summary>
/// Represents an arbitrary version.
/// </summary>
[Serializable]
public readonly struct LariVersion
{
    [XmlAttribute("major")]
    public uint Major { get; init; }

    [XmlAttribute("minor")]
    public uint Minor { get; init; }

    [XmlAttribute("revision")]
    public uint Revision { get; init; }

    [XmlAttribute("build")]
    public uint Build { get; init; }

    public static LariVersion FromUInt64(ulong version) => new()
    {
        Major = (uint)((version >> 55) & 0x7f),
        Minor = (uint)((version >> 47) & 0xff),
        Revision = (uint)((version >> 31) & 0xffff),
        Build = (uint)(version & 0x7fffffff),
    };

    public static LariVersion FromUInt32(uint version) => new()
    {
        Major = (version >> 28) & 0x0f,
        Minor = (version >> 24) & 0x0f,
        Revision = (version >> 16) & 0xff,
        Build = version & 0xffff,
    };

    public static ulong ToVersion64(LariVersion version) =>
        (ulong)(((long)version.Major & 0x7f) << 55 |
                ((long)version.Minor & 0xff) << 47 |
                ((long)version.Revision & 0xffff) << 31 |
                ((long)version.Build & 0x7fffffff) << 0);

    public static uint ToVersion32(LariVersion version) =>
        (version.Major & 0x0f) << 28 |
        (version.Minor & 0x0f) << 24 |
        (version.Revision & 0xff) << 16 |
        (version.Build & 0xffff) << 0;

    public override string ToString() => $"{Major}.{Minor}.{Revision}.{Build}";

    public static implicit operator LariVersion(ulong version) => FromUInt64(version);
    public static implicit operator LariVersion(uint version) => FromUInt32(version);
    public static implicit operator ulong(LariVersion version) => ToVersion64(version);
    public static implicit operator uint(LariVersion version) => ToVersion32(version);
}
