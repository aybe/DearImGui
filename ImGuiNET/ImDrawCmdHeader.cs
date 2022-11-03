namespace ImGuiNET;

partial class ImDrawCmdHeader
{
    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(ClipRect)}: {ClipRect}, {nameof(VtxOffset)}: {VtxOffset}, {nameof(TextureId)}: {TextureId}";
    }
}