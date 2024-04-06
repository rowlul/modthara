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
    public uint Version { get; internal set; }
    public ulong FileListOffset { get; internal set; }
    public uint FileListSize { get; internal set; }
    public byte Flags { get; internal set; }
    public byte Priority { get; internal set; }

    [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public byte[] Md5 { get; internal set; }

    public ushort NumParts { get; internal set; }
}
