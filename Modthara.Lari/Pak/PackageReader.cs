using System.Runtime.InteropServices;
using System.Text;

using K4os.Compression.LZ4;

using Modthara.Lari.Extensions;

namespace Modthara.Lari.Pak;

/// <summary>
/// Represents the reader of LSPK packages.
/// </summary>
public static class PackageReader
{
    /// <summary>
    /// Creates a new instance of <see cref="Package"/>.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <returns>Instance of <see cref="Package"/>.</returns>
    /// <exception cref="LspkException">
    /// Throws upon encountering invalid package structure or unsupported features.
    /// </exception>
    public static Package FromStream(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        stream.Seek(0, SeekOrigin.Begin);
        var signature = reader.ReadBytes(4);
        if (!Signature.SequenceEqual(signature))
        {
            throw new LspkException(
                $"Invalid package signature: " +
                $"expected {Convert.ToHexString(Signature)}, got {Convert.ToHexString(signature)}.");
        }

        var version = reader.ReadInt32();
        stream.Seek(4, SeekOrigin.Begin);
        if (version != Version)
        {
            throw new LspkException($"Unsupported package version: expected {Version}, got {version}.");
        }

        var header = ReadHeader(reader);
        var files = EnumerateFiles(reader, header);
        var package = new Package(header, files);
        return package;
    }

    private static LspkHeader ReadHeader(BinaryReader reader)
    {
        var header = reader.ReadStruct<LspkHeader>();
        if (header.NumParts > 1)
        {
            throw new LspkException("Multipart packages are not supported at this moment.");
        }

        return header;
    }

    private static List<PackagedFile> EnumerateFiles(BinaryReader reader, LspkHeader header)
    {
        reader.BaseStream.Seek((long)header.FileListOffset, SeekOrigin.Begin);
        var numFiles = reader.ReadInt32();
        var compressedSize = reader.ReadInt32();
        var compressedFileList = reader.ReadBytes(compressedSize);
        var fileBufferSize = Marshal.SizeOf(typeof(LspkEntry)) * numFiles;
        var uncompressedList = new byte[fileBufferSize];
        var uncompressedSize = LZ4Codec.Decode(compressedFileList, 0,
            compressedFileList.Length, uncompressedList, 0, fileBufferSize);

        if (uncompressedSize != fileBufferSize)
        {
            throw new LspkException(
                $"File header size mismatch: expected {fileBufferSize}, got {uncompressedSize}.");
        }

        var memoryStream = new MemoryStream(uncompressedList);
        var memoryStreamReader = new BinaryReader(memoryStream);
        var entries = new LspkEntry[numFiles];
        memoryStreamReader.ReadStructs(entries);

        List<PackagedFile> files = [];
        foreach (var entry in entries)
        {
            var file = new PackagedFile(reader.BaseStream, entry);
            var compressionMethod = file.Flags & 0x0F;
            if (compressionMethod > 2 || (file.Flags & ~0x7F) != 0)
            {
                throw new LspkException($"Packaged file '{file.Name}' has unsupported flags: {file.Flags}.");
            }

            files.Add(file);
        }

        return files;
    }

    /// <summary>
    /// Represents valid LSPK package signature.
    /// </summary>
    private static readonly byte[] Signature = [0x4C, 0x53, 0x50, 0x4B];

    /// <summary>
    /// Represents package version for Baldur's Gate 3 Release.
    /// </summary>
    private const uint Version = 0x12;
}
