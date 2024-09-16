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
            new LariVersion(144961330997915191)
        ),
        new Module(
            "ModtharaTest",
            "",
            "Modthara test mod",
            new LariUuid("adc05d68-6d4e-4763-9724-ac47bfb68c7b"),
            new LariVersion(36028797018963968)
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
    public void Remove_AllMods_ReturnsTrue()
    {
        var expected = new ModSettings();

        var actual = new ModSettings(Mods);
        var ret1 = actual.Remove(Mods[0]);
        var ret2 = actual.Remove(Mods[1]);

        actual.Should().BeEquivalentTo(expected);
        ret1.Should().BeTrue();
        ret2.Should().BeTrue();
    }

    [Fact]
    public void Remove_MissingMod_ReturnsFalse()
    {
        var mods = Mods.Take(1).ToList();

        var expected = new ModSettings(mods);

        var actual = new ModSettings(mods); // must not change
        var ret = actual.Remove(Mods[1]);

        actual.Should().BeEquivalentTo(expected);
        ret.Should().BeFalse();
    }
}
