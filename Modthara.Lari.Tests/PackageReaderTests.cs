using System.IO.Abstractions;

using Modthara.Lari.Lsx;
using Modthara.Lari.Pak;

namespace Modthara.Lari.Tests;

public class PackageReaderTests
{
    [Fact]
    public void FromStream_ValidHeader_ReturnsPackage()
    {
        var fs = new FileSystem();
        var pak = PackageReader.FromStream(fs.File.OpenRead("./TestFiles/Sample_VFX_Mod.pak"));
        pak.FileListOffset.Should().Be(0x144E8);
        pak.FileListSize.Should().Be(0x1D3);
        pak.Flags.Should().Be(0x0);
        pak.NumParts.Should().Be(0x1);
        pak.Priority.Should().Be(0x1E);
        pak.Version.Should().Be(0x12);
    }

    [Fact]
    public void FromStream_ValidPackagedFile_ReturnsPackage()
    {
        var fs = new FileSystem();
        var file = PackageReader.FromStream(fs.File.OpenRead("./TestFiles/Sample_VFX_Mod.pak")).Files[1];
        file.ArchivePart.Should().Be(0x0);
        file.Flags.Should().Be(0x12);
        file.IsDeleted.Should().Be(false);
        file.Name.Should().Be("Mods/Sample_VFX_Mod/meta.lsx");
        file.OffsetInFile.Should().Be(0x168);
        file.Size.Should().Be(0x815);
        file.SizeOnDisk.Should().Be(0x32B);
        file.UncompressedSize.Should().Be(0x815);
    }

    [Fact]
    public void FromStream_ValidLsxInPackageFile_ReturnsPackage()
    {
        var fs = new FileSystem();
        var file = PackageReader.FromStream(fs.File.OpenRead("./TestFiles/Sample_VFX_Mod.pak")).Files[1];
        var lsx = () =>
        {
            using var stream = file.Open();
            return LsxDocument.FromStream(stream);
        };
        lsx.Should().NotThrow();
    }

    [Fact]
    public void FromStream_UnsupportedVersion_ThrowsException()
    {
        var fs = new FileSystem();
        var s = () => PackageReader.FromStream(fs.File.OpenRead("./TestFiles/mod_v15.pak"));
        s.Should().Throw<LspkException>().WithMessage("Unsupported package version: expected 18, got 15.");
    }
}
