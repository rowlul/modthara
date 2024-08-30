using System.Xml.Serialization;

namespace Modthara.Lari;

/// <summary>
/// Represents an arbitrary version.
/// </summary>
[Serializable]
public readonly struct LariVersion : IComparable<LariVersion>, IEquatable<LariVersion>
{
    public static readonly LariVersion Empty = new();

    public bool IsEmpty => Major == 0 && Minor == 0 && Revision == 0 && Build == 0;

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
        Major = (uint)((version >> 55) & 0x7f);
        Minor = (uint)((version >> 47) & 0xff);
        Revision = (uint)((version >> 31) & 0xffff);
        Build = (uint)(version & 0x7fffffff);
    }

    public LariVersion(uint major, uint minor, uint revision, uint build)
    {
        Major = major;
        Minor = minor;
        Revision = revision;
        Build = build;
    }

    public ulong ToUInt64() =>
        (ulong)(((long)Major & 0x7f) << 55 |
                ((long)Minor & 0xff) << 47 |
                ((long)Revision & 0xffff) << 31 |
                ((long)Build & 0x7fffffff) << 0);

    public static bool operator ==(LariVersion a, LariVersion b) => a.Major == b.Major &&
                                                                    a.Minor == b.Minor &&
                                                                    a.Revision == b.Revision &&
                                                                    a.Build == b.Build;

    public static bool operator !=(LariVersion a, LariVersion b) => a.Major != b.Major &&
                                                                    a.Minor != b.Minor &&
                                                                    a.Revision != b.Revision &&
                                                                    a.Build != b.Build;

    /// <inheritdoc />
    public bool Equals(LariVersion other) => this == other;

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
    public override bool Equals(object? obj) => obj is LariVersion other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Major, Minor, Revision, Build);

    /// <inheritdoc />
    public override string ToString() => $"{Major}.{Minor}.{Revision}.{Build}";
}
