using System.Xml.Serialization;

namespace Modthara.Lari;

/// <summary>
/// Represents an arbitrary version.
/// </summary>
[Serializable]
public readonly struct LariVersion : IComparable<LariVersion>, IComparable<ulong>, IComparable<uint>
{
    public static LariVersion Default = new(1, 0, 0, 0);

    [XmlAttribute("major")]
    public uint Major { get; init; }

    [XmlAttribute("minor")]
    public uint Minor { get; init; }

    [XmlAttribute("revision")]
    public uint Revision { get; init; }

    [XmlAttribute("build")]
    public uint Build { get; init; }

    public LariVersion(ulong version)
    {
        this = FromUInt64(version);
    }

    public LariVersion(uint version)
    {
        this = FromUInt32(version);
    }

    public LariVersion(uint major, uint minor, uint revision, uint build)
    {
        Major = major;
        Minor = minor;
        Revision = revision;
        Build = build;
    }

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

    public static ulong ToUInt64(LariVersion version) =>
        (ulong)(((long)version.Major & 0x7f) << 55 |
                ((long)version.Minor & 0xff) << 47 |
                ((long)version.Revision & 0xffff) << 31 |
                ((long)version.Build & 0x7fffffff) << 0);

    public ulong ToUInt64() => ToUInt64(this);

    public static uint ToUInt32(LariVersion version) =>
        (version.Major & 0x0f) << 28 |
        (version.Minor & 0x0f) << 24 |
        (version.Revision & 0xff) << 16 |
        (version.Build & 0xffff) << 0;

    public uint ToUInt32() => ToUInt32(this);

    /// <inheritdoc />
    public int CompareTo(LariVersion other)
    {
        if (Major != other.Major)
            return Major.CompareTo(other.Major);
        if (Minor != other.Minor)
            return Minor.CompareTo(other.Minor);
        if (Revision != other.Revision)
            return Revision.CompareTo(other.Revision);
        return Build.CompareTo(other.Build);
    }

    /// <inheritdoc />
    public int CompareTo(ulong other)
    {
        return CompareTo(FromUInt64(other));
    }

    /// <inheritdoc />
    public int CompareTo(uint other)
    {
        return CompareTo(FromUInt32(other));
    }

    public override string ToString() => $"{Major}.{Minor}.{Revision}.{Build}";

    public static implicit operator LariVersion(ulong version) => FromUInt64(version);
    public static implicit operator LariVersion(uint version) => FromUInt32(version);
    public static implicit operator ulong(LariVersion version) => ToUInt64(version);
    public static implicit operator uint(LariVersion version) => ToUInt32(version);
}
