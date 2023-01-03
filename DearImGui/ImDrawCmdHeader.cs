using JetBrains.Annotations;

namespace DearImGui;

/// <summary>
///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L2433
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
partial class ImDrawCmdHeader
{
    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(ClipRect)}: {ClipRect}, {nameof(VtxOffset)}: {VtxOffset}, {nameof(TextureId)}: {TextureId}";
    }
}