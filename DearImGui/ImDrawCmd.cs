using System.Numerics;
using JetBrains.Annotations;

namespace DearImGui;

/// <summary>
///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L2400
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
partial struct ImDrawCmd
{
    /// <summary>
    ///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L2402.
    /// </summary>
    public Vector4 ClipRect => __instance.ClipRect;

    /// <summary>
    ///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L2413
    /// </summary>
    public IntPtr GetTexID()
    {
        // TODO keep synchronized with value in header as it may change
        return __Instance.TextureId;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return
            $"{nameof(ClipRect)}: {ClipRect}, {nameof(ElemCount)}: {ElemCount}, {nameof(VtxOffset)}: {VtxOffset}, {nameof(IdxOffset)}: {IdxOffset}, {nameof(TextureId)}: {TextureId}";
    }
}