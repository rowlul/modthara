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
    public void ToVersion64_ReturnsVersion()
    {
        const ulong expected = 36028797018963968;
        var actual = LariVersion.ToVersion64(new LariVersion { Major = 1, Minor = 0, Build = 0, Revision = 0 });
        actual.Should().Be(expected);
    }

    [Fact]
    public void FromVersion32_ReturnsVersion()
    {
        const int expected = 268435456;
        var actual = LariVersion.ToVersion32(new LariVersion { Major = 1, Minor = 0, Build = 0, Revision = 0 });
        actual.Should().Be(expected);
    }
}
