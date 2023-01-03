using System.Runtime.InteropServices;
using OpenTK.Mathematics;
using Vector4 = System.Numerics.Vector4;

#pragma warning disable CS1591

namespace DearImGui.OpenTK.Extensions;

public static class SpanExtensions
{
    public static Span<float> AsSpan(this ref Color4 source)
    {
        return MemoryMarshal.CreateSpan(ref source.R, 4);
    }

    public static Vector4 ToVector4(this Color4 source)
    {
        return new Vector4(source.AsSpan());
    }
}