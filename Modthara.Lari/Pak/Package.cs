namespace Modthara.Lari.Pak;

/// <summary>
/// Represents an LSPK package.
/// </summary>
public class Package
{
    /// <summary>
    /// <inheritdoc cref="Package"/>
    /// </summary>
    /// <param name="header">Header of the package that has previously been marshalled.</param>
    /// <param name="files">List of packages files that the package contains.</param>
    internal Package(LspkHeader header, IReadOnlyList<PackagedFile> files)
    {
        Version = header.Version;
        FileListOffset = header.FileListOffset;
        FileListSize = header.FileListSize;
        NumParts = header.NumParts;
        Flags = header.Flags;
        Priority = header.Priority;
        Md5 = header.Md5;
        Files = files;
    }

    internal uint Version { get; private set; }
    internal ulong FileListOffset { get; private set; }
    internal uint FileListSize { get; private set; }
    internal uint NumParts { get; private set; }
    internal uint Flags { get; private set; }
    internal uint Priority { get; private set; }
    internal byte[] Md5 { get; private set; }
    public IReadOnlyList<PackagedFile> Files { get; set; }
}
