using Modthara.Lari.Lsx;
using Modthara.Lari.Pak;

namespace Modthara.Lari.Tests;

public class PackageReaderTests
{
    [Fact]
    public void FromStream_ValidHeader_ReturnsPackage()
    {
        using var s = File.OpenRead("./TestFiles/Sample_VFX_Mod.pak");

        var pak = PackageReader.FromStream(s);
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
        using var s = File.OpenRead("./TestFiles/Sample_VFX_Mod.pak");

        var file = PackageReader.FromStream(s).Files[1];
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
        using var s = File.OpenRead("./TestFiles/Sample_VFX_Mod.pak");

        var file = PackageReader.FromStream(s).Files[1];
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
        var s = () => PackageReader.FromStream(File.OpenRead("./TestFiles/mod_v15.pak"));
        s.Should().Throw<LspkException>().WithMessage("Unsupported package version: expected 18, got 15.");
    }
}
