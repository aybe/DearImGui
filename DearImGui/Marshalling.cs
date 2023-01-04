using System.Runtime.CompilerServices;

namespace DearImGui;

internal static class Marshalling
{
    public static bool BitGet(uint val, int bit)
    {
        return (val & (1u << bit)) != 0;
    }

    public static uint BitSet(uint val, int bit, bool set)
    {
        var msk = 1u << bit;

        if (set)
        {
            val |= msk;
        }
        else
        {
            val &= ~msk;
        }

        return val;
    }

    public static unsafe void Copy<T>(ref T[] destination, in void* source) where T : struct
    {
        var memory = destination.AsMemory();

        using var handle = memory.Pin();

        var sizeOf = Unsafe.SizeOf<T>();
        var length = sizeOf * destination.Length;

        Unsafe.CopyBlock(handle.Pointer, source, (uint)length);
    }

    public static unsafe void Copy<T>(in void* destination, ref T[] source) where T : struct
    {
        var memory = source.AsMemory();

        using var handle = memory.Pin();

        var sizeOf = Unsafe.SizeOf<T>();
        var length = sizeOf * source.Length;

        Unsafe.CopyBlock(destination, handle.Pointer, (uint)length);
    }

    public static unsafe string ReadString(sbyte* value)
    {
        var @string = new string(value).TrimEnd('\0');

        return @string;
    }

    public static ImVector<T> ReadVector<T>(ref ImVector.__Internal source)
    {
        var @internal = Unsafe.As<ImVector.__Internal, ImVector<T>.__Internal>(ref source);

        var vector = new ImVector<T>(@internal);

        return vector;
    }
}