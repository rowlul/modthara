namespace Modthara.Lari.Tests;

public class ModMetadataTests
{
    [Fact]
    public void Constructor_LsxDocumentWithExampleMeta_ReturnsMod()
    {
        using var s = File.OpenRead("./TestFiles/meta.lsx");
        var lsx = LsxDocument.FromStream(s);

        var expected = new ModMetadata
        (
            new LariUuid("adc05d68-6d4e-4763-9724-ac47bfb68c7b"),
            "ModtharaTest",
            new LariVersion(36028797018963968UL),
            "",
            "Modthara test mod",
            "rowlul",
            "Modthara test mod",
            [
                new Module
                (
                    "SharedDev",
                    "",
                    "SharedDev",
                    new LariUuid("3d0c5ff8-c95d-c907-ff3e-34b204f1c630"),
                    new LariVersion(36028797022722506UL)
                )
            ]
        );

        var actual = new ModMetadata(lsx);
        actual.Should().BeEquivalentTo(expected);
    }
}
