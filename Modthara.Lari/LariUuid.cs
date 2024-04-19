namespace Modthara.Lari;

/// <summary>
/// Wrapper for <see cref="System.Guid"/> that accepts non-valid values.
/// </summary>
public readonly struct LariUuid(string uuid)
{
    public string Uuid { get; init; } = uuid;

    public static LariUuid NewGuid() => new(new Guid().ToString());

    public bool TryParse(out Guid guid) => Guid.TryParse(Uuid, out guid);

    public static bool operator ==(LariUuid a, LariUuid b) => a.Uuid == b.Uuid;

    public static bool operator !=(LariUuid a, LariUuid b) => a.Uuid != b.Uuid;

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

    public bool Equals(LariUuid other)
    {
        return Uuid == other.Uuid;
    }

    public override bool Equals(object? obj)
    {
        return obj is LariUuid other && Equals(other);
    }

    public override int GetHashCode()
    {
        if (this.TryParse(out Guid guid))
        {
            return guid.GetHashCode();
        }

        return Uuid.GetHashCode();
    }
}
