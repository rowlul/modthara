using System.Text;

namespace Modthara.Lari.Pak;

/// <summary>
/// Represents an arbitrary packaged filed in an LSPK package.
/// </summary>
public class PackagedFile
{
    public string Name { get; private set; }
    internal uint ArchivePart { get; private set; }
    internal byte Flags { get; private set; }
    internal ulong OffsetInFile { get; private set; }
    internal ulong SizeOnDisk { get; private set; }
    internal ulong UncompressedSize { get; private set; }

    /// <summary>
    /// <inheritdoc cref="PackagedFile"/>
    /// </summary>
    /// <param name="entry">File entry that has previously been marshalled.</param>
    internal PackagedFile(LspkEntry entry)
    {
        int l;
        for (l = 0; l < entry.Name.Length && entry.Name[l] != 0; l++) { }

        var name = Encoding.UTF8.GetString(entry.Name, 0, l);
        var offset = entry.OffsetInFile1 | ((ulong)entry.OffsetInFile2 << 32);

        Name = name;
        ArchivePart = entry.ArchivePart;
        Flags = entry.Flags;
        OffsetInFile = offset;
        SizeOnDisk = entry.SizeOnDisk;
        UncompressedSize = entry.UncompressedSize;
    }
}
