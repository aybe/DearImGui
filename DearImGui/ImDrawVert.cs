using System.Numerics;
using JetBrains.Annotations;

// ReSharper disable IdentifierTypo

namespace DearImGui;

/// <summary>
///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L140
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public struct ImDrawVert
{
    /// <summary>
    ///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L2420
    /// </summary>
    public Vector2 Pos;

    /// <summary>
    ///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L2421
    /// </summary>
    public Vector2 Uv;

    /// <summary>
    ///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L2422
    /// </summary>
    public uint Col;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(Pos)}: {Pos}, {nameof(Uv)}: {Uv}, {nameof(Col)}: 0x{Col:X8}";
    }
}