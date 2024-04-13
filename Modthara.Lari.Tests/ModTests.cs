using System.IO.Abstractions;

using Modthara.Lari.Lsx;

namespace Modthara.Lari.Tests;

public class ModTests
{
    [Fact]
    public void FromLsx_ExampleMeta_ReturnsMod()
    {
        var fs = new FileSystem();
        using var s = fs.File.OpenRead("./TestFiles/meta.lsx");
        var lsx = LsxDocument.FromStream(s);

        var expected = new Mod
        {
            Name = "Modthara test mod",
            Author = "rowlul",
            Description = "Modthara test mod",
            FolderName = "ModtharaTest",
            Md5 = "",
            Uuid = new Guid("adc05d68-6d4e-4763-9724-ac47bfb68c7b"),
            Version = 36028797018963968UL,
            Dependencies = [
                new Mod
                {
                    FolderName = "SharedDev",
                    Md5 = "",
                    Name = "SharedDev",
                    Uuid = new Guid("3d0c5ff8-c95d-c907-ff3e-34b204f1c630"),
                    Version = 36028797022722506UL
                }
            ]
        };

        var actual = Mod.FromLsx(lsx);
        actual.Should().BeEquivalentTo(expected);
    }
}
