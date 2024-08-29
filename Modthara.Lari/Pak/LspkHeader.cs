using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Modthara.Lari.Pak;

/// <summary>
/// Represents the header in an LSPK package.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global",
    Justification = "Struct is marshalled via an internal method.")]
internal struct LspkHeader
{
    public uint Version;
    public ulong FileListOffset;
    public uint FileListSize;
    public byte Flags;
    public byte Priority;

    [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public byte[] Md5;

    public ushort NumParts;
}
