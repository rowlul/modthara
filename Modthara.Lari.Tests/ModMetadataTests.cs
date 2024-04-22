using Modthara.Lari.Lsx;

namespace Modthara.Lari.Tests;

public class ModMetadataTests
{
    [Fact]
    public void FromLsx_ExampleMeta_ReturnsMod()
    {
        using var s = File.OpenRead("./TestFiles/meta.lsx");
        var lsx = LsxDocument.FromStream(s);

        var expected = new ModMetadata
        {
            Name = "Modthara test mod",
            Author = "rowlul",
            Description = "Modthara test mod",
            FolderName = "ModtharaTest",
            Md5 = "",
            Uuid = new LariUuid("adc05d68-6d4e-4763-9724-ac47bfb68c7b"),
            Version = 36028797018963968UL,
            Dependencies =
            [
                new ModMetadata
                {
                    FolderName = "SharedDev",
                    Md5 = "",
                    Name = "SharedDev",
                    Uuid = new LariUuid("3d0c5ff8-c95d-c907-ff3e-34b204f1c630"),
                    Version = 36028797022722506UL
                }
            ]
        };

        var actual = ModMetadata.FromLsx(lsx);
        actual.Should().BeEquivalentTo(expected);
    }
}
