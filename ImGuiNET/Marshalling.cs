using System.Runtime.CompilerServices;

namespace imgui.NET;

public static class Marshalling
{
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
}