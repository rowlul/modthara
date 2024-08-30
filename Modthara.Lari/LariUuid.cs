namespace Modthara.Lari;

/// <summary>
/// Wrapper for <see cref="System.Guid"/> that accepts non-valid values.
/// </summary>
public sealed class LariUuid : IEquatable<LariUuid>
{
    public readonly string Value;

    public LariUuid(string guid)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(guid);

        Value = guid;
    }

    public bool TryParse(out Guid guid) => Guid.TryParse(Value, out guid);

    public static LariUuid NewGuid() => new(new Guid().ToString());

    public static bool operator ==(LariUuid a, LariUuid b) => a.Value == b.Value;

    public static bool operator !=(LariUuid a, LariUuid b) => a.Value != b.Value;

    public static bool operator ==(LariUuid a, Guid b)
    {
        Guid aGuid;
        if (!a.TryParse(out aGuid))
        {
            return false;
        }

        return aGuid == b;
    }

    public static bool operator !=(LariUuid a, Guid b)
    {
        Guid aGuid;
        if (!a.TryParse(out aGuid))
        {
            return true;
        }

        return aGuid != b;
    }

    public static bool operator ==(Guid a, LariUuid b)
    {
        Guid bGuid;
        if (!b.TryParse(out bGuid))
        {
            return false;
        }

        return a == bGuid;
    }

    public static bool operator !=(Guid a, LariUuid b)
    {
        Guid bGuid;
        if (!b.TryParse(out bGuid))
        {
            return true;
        }

        return a != bGuid;
    }

    /// <inheritdoc />
    public bool Equals(LariUuid other) => this == other;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is LariUuid other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => this.TryParse(out Guid guid) ? guid.GetHashCode() : Value.GetHashCode();

    /// <inheritdoc />
    public override string ToString() => Value;
}
