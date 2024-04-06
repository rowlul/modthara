using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Modthara.Lari.Pak;

/// <summary>
/// Represents an arbitrary file entry in an LSPK package.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global",
    Justification = "Struct is marshalled via an internal method.")]
internal struct LspkEntry
{
    [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
    public byte[] Name { get; internal set; }

    public uint OffsetInFile1 { get; internal set; }
    public ushort OffsetInFile2 { get; internal set; }
    public byte ArchivePart { get; internal set; }
    public byte Flags { get; internal set; }
    public uint SizeOnDisk { get; internal set; }
    public uint UncompressedSize { get; internal set; }
}
