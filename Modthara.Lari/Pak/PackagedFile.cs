using System.Text;

namespace Modthara.Lari.Pak;

/// <summary>
/// Represents an arbitrary packaged filed in an LSPK package.
/// </summary>
public class PackagedFile : IDisposable
{
    private Stream? _stream;

    public Stream OwnerStream { get; private set; }
    public string Name { get; private set; }
    internal uint ArchivePart { get; private set; }
    internal byte Flags { get; private set; }
    internal ulong OffsetInFile { get; private set; }
    internal ulong SizeOnDisk { get; private set; }
    internal ulong UncompressedSize { get; private set; }

    public bool IsDeleted => (OffsetInFile & 0x0000ffffffffffff) == 0xbeefdeadbeef;

    /// <summary>
    /// <inheritdoc cref="PackagedFile"/>
    /// </summary>
    /// <param name="ownerStream">Stream of the package that this file belongs to.</param>
    /// <param name="entry">File entry that has previously been marshalled.</param>
    internal PackagedFile(Stream ownerStream, LspkEntry entry)
    {
        int l;
        for (l = 0; l < entry.Name.Length && entry.Name[l] != 0; l++) { }

        var name = Encoding.UTF8.GetString(entry.Name, 0, l);
        var offset = entry.OffsetInFile1 | ((ulong)entry.OffsetInFile2 << 32);

        OwnerStream = ownerStream;
        Name = name;
        ArchivePart = entry.ArchivePart;
        Flags = entry.Flags;
        OffsetInFile = offset;
        SizeOnDisk = entry.SizeOnDisk;
        UncompressedSize = entry.UncompressedSize;
    }

    public Stream Open()
    {
        if (IsDeleted)
        {
            throw new LspkException($"Cannot open deleted file '${Name}'.");
        }

        if ((Flags & 0x0F) != 0x0)
        {
            throw new LspkException($"File {Name} is compressed which is not supported at this moment.");
        }

        OwnerStream.Seek((long)OffsetInFile, SeekOrigin.Begin);
        _stream = new PackagedFileStream(OwnerStream, this);
        return _stream;
    }

    public void Dispose()
    {
        _stream?.Dispose();
        _stream = null;
        GC.SuppressFinalize(this);
    }
}
