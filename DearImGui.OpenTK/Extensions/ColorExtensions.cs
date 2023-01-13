using System.Drawing;
using System.Numerics;

#pragma warning disable CS1591

namespace DearImGui.OpenTK.Extensions;

public static class ColorExtensions
{
    public static Vector4 ToVector4(this Color color)
    {
        const float f = 1.0f / 255.0f;

        var vector4 = new Vector4(color.R * f, color.G * f, color.B * f, color.A * f);

        return vector4;
    }
}