using System.Runtime.InteropServices;

namespace Modthara.Lari.Extensions;

internal static class StructExtensions
{
    public static TStruct ReadStruct<TStruct>(this BinaryReader reader) where TStruct : struct
    {
        var size = Marshal.SizeOf(typeof(TStruct));
        var buffer = reader.ReadBytes(size);
        var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        var outStruct = (TStruct)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(TStruct))!;
        handle.Free();

        return outStruct;
    }

    public static void ReadStructs<TStruct>(this BinaryReader reader, TStruct[] structs) where TStruct : struct
    {
        var size = Marshal.SizeOf(typeof(TStruct));
        var bytes = size * structs.Length;
        var buffer = reader.ReadBytes(bytes);
        var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        var address = handle.AddrOfPinnedObject();

        for (var i = 0; i < structs.Length; i++)
        {
            IntPtr structAddress = new(address.ToInt64() + (size * i));
            structs[i] = Marshal.PtrToStructure<TStruct>(structAddress);
        }

        handle.Free();
    }
}
