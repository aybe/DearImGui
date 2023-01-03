using System.Numerics;
using System.Runtime.InteropServices;

#pragma warning disable CS1591

namespace DearImGui;

public static class SpanExtensions
{
    public static Span<float> AsSpan(this ref Vector2 source)
    {
        return MemoryMarshal.CreateSpan(ref source.X, 2);
    }

    public static Span<float> AsSpan(this ref Vector3 source)
    {
        return MemoryMarshal.CreateSpan(ref source.X, 3);
    }

    public static Span<float> AsSpan(this ref Vector4 source)
    {
        return MemoryMarshal.CreateSpan(ref source.X, 4);
    }
}