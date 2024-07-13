namespace Modthara.Lari.Tests;

public class LariVersionTests
{
    [Fact]
    public void FromUInt64_ReturnsVersion()
    {
        var expected = new LariVersion { Major = 1, Minor = 0, Build = 0, Revision = 0 };
        var actual = LariVersion.FromUInt64(36028797018963968UL);
        actual.Should().Be(expected);
    }

    [Fact]
    public void FromUInt32_ReturnsVersion()
    {
        var expected = new LariVersion { Major = 1, Minor = 0, Build = 0, Revision = 0 };
        var actual = LariVersion.FromUInt32(268435456);
        actual.Should().Be(expected);
    }

    [Fact]
    public void ToUInt64_ReturnsUInt64()
    {
        const ulong expected = 36028797018963968;
        var actual = LariVersion.ToUInt64(new LariVersion { Major = 1, Minor = 0, Build = 0, Revision = 0 });
        actual.Should().Be(expected);
    }

    [Fact]
    public void ToUInt32_ReturnsUInt32()
    {
        const int expected = 268435456;
        var actual = LariVersion.ToUInt32(new LariVersion { Major = 1, Minor = 0, Build = 0, Revision = 0 });
        actual.Should().Be(expected);
    }
}
