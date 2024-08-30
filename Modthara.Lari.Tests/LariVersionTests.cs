namespace Modthara.Lari.Tests;

public class LariVersionTests
{
    [Fact]
    public void FromUInt64_ReturnsVersion()
    {
        var expected = new LariVersion { Major = 1, Minor = 0, Build = 0, Revision = 0 };
        var actual = new LariVersion(36028797018963968UL);
        actual.Should().Be(expected);
    }

    [Fact]
    public void ToUInt64_ReturnsUInt64()
    {
        const ulong expected = 36028797018963968;
        var actual = new LariVersion { Major = 1, Minor = 0, Build = 0, Revision = 0 }.ToUInt64();
        actual.Should().Be(expected);
    }
}
