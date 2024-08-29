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
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
    public byte[] Name;

    public uint OffsetInFile1;
    public ushort OffsetInFile2;
    public byte ArchivePart;
    public byte Flags;
    public uint SizeOnDisk;
    public uint UncompressedSize;
}
