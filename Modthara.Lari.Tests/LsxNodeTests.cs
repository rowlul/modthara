using Modthara.Lari.Lsx;

namespace Modthara.Lari.Tests;

public class LsxNodeTests
{
    [Theory]
    [InlineData(0, "one")]
    [InlineData(1, "two")]
    [InlineData(2, "three")]
    public void GetAttributeValue_ExistingAttributeNoDefaultValue_ReturnsString(int index, string id)
    {
        var node = new LsxNode
        {
            Id = "Test",
            Attributes =
            [
                new LsxAttribute { Id = "one", Type = "string", Value = "val1" },
                new LsxAttribute { Id = "two", Type = "string", Value = "val2" },
                new LsxAttribute { Id = "three", Type = "string", Value = "val3" }
            ]
        };

        var expected = node.Attributes[index].Value;
        var actual = node.GetAttributeValue(id);
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("one")]
    [InlineData("two")]
    public void GetAttributeValue_EmptyOrNullAttribute_ThrowsException(string id)
    {
        var node = new LsxNode
        {
            Id = "Test",
            Attributes =
            [
                new LsxAttribute { Id = "one", Type = "string", Value = "" }
            ]
        };

        var attribute = () => node.GetAttributeValue(id);
        attribute.Should().Throw<LsxMissingElementException>();
    }

    [Fact]
    public void GetAttributeValue_ExistingAttributeDefaultValue_ReturnsString()
    {
        var node = new LsxNode { Id = "Test", Attributes = null };

        var expected = "default";
        var actual = node.GetAttributeValue("attr", "default");
        actual.Should().Be(expected);
    }

    [Fact]
    public void GetUuid_ExistingAttribute_ReturnsLarUuid()
    {
        var node = new LsxNode
        {
            Id = "Test",
            Attributes =
            [
                new LsxAttribute { Id = "UUID", Type = "FixedString", Value = "adc05d68-6d4e-4763-9724-ac47bfb68c7b" }
            ]
        };

        var expected = new LariUuid("adc05d68-6d4e-4763-9724-ac47bfb68c7b");
        var actual = node.GetUuid();
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("Version")]
    [InlineData("Version64")]
    [InlineData("Version32")]
    public void GetVersion_ExistingAttribute_ReturnsVersion(string id)
    {
        var node = new LsxNode
        {
            Id = "Test",
            Attributes =
            [
                new LsxAttribute { Id = id, Type = "int64", Value = "36028797018963968" }
            ]
        };

        var expected = new LariVersion(36028797018963968);
        var actual = node.GetVersion();
        actual.Should().Be(expected);
    }
}
