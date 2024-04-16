using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace Modthara.Lari.Tests;

public class ModSettingsTests
{
    private static readonly List<Mod> Mods =
    [
        new Mod
        {
            FolderName = "GustavDev",
            Md5 = "e41378521136e6646b5491305373f538",
            Name = "GustavDev",
            Uuid = new Guid("28ac9ce2-2aba-8cda-b3b5-6e922f71b6b8"),
            Version = 144961330997915191,
            IsEnabled = true
        },
        new Mod
        {
            FolderName = "ModtharaTest",
            Md5 = "",
            Name = "Modthara test mod",
            Uuid = new Guid("adc05d68-6d4e-4763-9724-ac47bfb68c7b"),
            Version = 36028797018963968,
            IsEnabled = true
        }
    ];

    [Fact]
    public void Read_SanitizedLsx_ReturnsModSettings()
    {
        var expected = new ModSettings(new LariVersion(4, 0, 9, 331), Mods);

        var fs = new FileSystem();
        var actual = ModSettings.Read("./TestFiles/modsettings_sanitized.lsx", fs.FileStream);
        actual.Should().BeEquivalentTo(expected,
            o => o.For(m => m.Mods).Exclude(x => x.IsEnabled));
    }

    [Fact]
    public void Write_EmptyOrder_CreatesModSettingsFile()
    {
        const string expected =
            """
            <?xml version="1.0" encoding="UTF-8"?>
            <save>
                <version major="0" minor="0" revision="0" build="0" />
                <region id="ModuleSettings">
                    <node id="root">
                        <children>
                            <node id="ModOrder" />
                            <node id="Mods" />
                        </children>
                    </node>
                </region>
            </save>
            """;

        var fs = new MockFileSystem();
        var path = @"c:\modsettings.lsx";
        var order = new ModSettings();
        order.Write(path, fs.FileStream);

        var actual = fs.File.ReadAllText(path).ReplaceLineEndings("\n");
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("duplicates")]
    [InlineData("order")]
    public void Sanitize_UnsanitizedLsx_ReturnsModSettings(string variation)
    {
        var expected = new ModSettings(new LariVersion(4, 0, 9, 331), Mods);

        var fs = new FileSystem();
        var actual = ModSettings.Read($"./TestFiles/modsettings_unsanitized_{variation}.lsx", fs.FileStream);
        actual.Sanitize();
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void FindOrdered_ExistingMod_ReturnsIndexModTuple()
    {
        var order = new ModSettings(new LariVersion(4, 0, 9, 331), Mods);
        var expected = (1, Mods[1]);
        var actual = order.FindOrdered(Mods[1].Uuid);
        actual.Should().BeEquivalentTo(expected, o => o.Excluding(x => x.Item2.IsEnabled));
    }

    [Fact]
    public void FindOrdered_MissingMod_ReturnsNullNullTuple()
    {
        var order = new ModSettings();
        (int?, Mod?) expected = (null, null);
        var actual = order.FindOrdered(Mods[1].Uuid);
        actual.Should().BeEquivalentTo(expected);
    }
}
