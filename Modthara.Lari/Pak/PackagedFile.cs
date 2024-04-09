using System.IO.Compression;
using System.Text;

using K4os.Compression.LZ4;

namespace Modthara.Lari.Pak;

/// <summary>
/// Represents an arbitrary packaged filed in an LSPK package.
/// </summary>
public class PackagedFile
{
    private Stream? _stream;

    public Stream OwnerStream { get; private set; }
    public string Name { get; private set; }
    internal uint ArchivePart { get; private set; }
    internal byte Flags { get; private set; }
    internal ulong OffsetInFile { get; private set; }
    internal ulong SizeOnDisk { get; private set; }
    internal ulong UncompressedSize { get; private set; }

    public ulong Size => (Flags & 0x0F) == 0 ? SizeOnDisk : UncompressedSize;
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

    /// <summary>
    /// Creates and opens stream of the file.
    /// </summary>
    /// <returns>Stream of the file</returns>
    /// <exception cref="LspkException">Throws if file is deleted or compression method is invalid.</exception>
    /// <exception cref="InvalidDataException">Throws if file size is mismatched.</exception>
    public Stream Open()
    {
        if (IsDeleted)
        {
            throw new LspkException($"Cannot open deleted file '${Name}'.");
        }

        OwnerStream.Seek((long)OffsetInFile, SeekOrigin.Begin);
        var compression = Flags & 0x0F;
        if (compression == 0x0)
        {
            _stream = new UncompressedPackagedFileStream(OwnerStream, this);
        }
        else
        {
            var compressedBytes = new byte[SizeOnDisk];
            var size = OwnerStream.Read(compressedBytes, 0, (int)SizeOnDisk);
            if (size != (long)SizeOnDisk)
            {
                throw new InvalidDataException($"Uncompressed file size mismatch: expected {SizeOnDisk}, got {size}.");
            }

            if (compression == 0x1)
            {
                _stream = new MemoryStream();
                using var compressedStream = new MemoryStream(compressedBytes);
                using var stream = new ZLibStream(compressedStream, CompressionMode.Decompress);
                stream.CopyTo(_stream);
            }
            else if (compression == 0x2)
            {
                var decompressedBytes = new byte[Size];
                LZ4Codec.Decode(compressedBytes, 0, compressedBytes.Length, decompressedBytes, 0, (int)Size);
                _stream = new MemoryStream(decompressedBytes);
            }
            else
            {
                throw new LspkException($"Invalid compression method: {compression}.");
            }
        }

        return _stream;
    }
}
