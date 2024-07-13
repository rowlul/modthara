using Modthara.Lari.Lsx;

namespace Modthara.Lari.Tests;

public class ModSettingsTests
{
    private static readonly List<Module> Mods =
    [
        new Module(
            "GustavDev",
            "e41378521136e6646b5491305373f538",
            "GustavDev",
            new LariUuid("28ac9ce2-2aba-8cda-b3b5-6e922f71b6b8"),
            144961330997915191
        ),
        new Module(
            "ModtharaTest",
            "",
            "Modthara test mod",
            new LariUuid("adc05d68-6d4e-4763-9724-ac47bfb68c7b"),
            36028797018963968
        )
    ];

    [Theory]
    [InlineData("duplicates")]
    [InlineData("order")]
    public void Sanitize_UnsanitizedLsx_ReturnsModSettings(string variation)
    {
        var expected = new ModSettings(Mods);

        using var s = File.OpenRead($"./TestFiles/modsettings_unsanitized_{variation}.lsx");
        var lsx = LsxDocument.FromStream(s);
        var actual = new ModSettings(lsx);

        actual.Sanitize();
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Find_ExistingMod_ReturnsIndexModTuple()
    {
        var order = new ModSettings(Mods);
        var expected = (1, Mods[1]);
        var actual = order.Find(Mods[1].Uuid);
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Find_MissingMod_ReturnsNullNullTuple()
    {
        var order = new ModSettings();
        (int?, ModMetadata?) expected = (null, null);
        var actual = order.Find(Mods[1].Uuid);
        actual.Should().BeEquivalentTo(expected);
    }
}
