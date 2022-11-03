namespace ImGuiNET;

partial class ImDrawCmd
{
    public unsafe IntPtr GetTexID()
    {
        // this is an inlined function and therefore has no entry point
        // TODO keep synchronized with value in header as it may change
        return ((__Internal*)__Instance)->TextureId;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(ElemCount)}: {ElemCount}, {nameof(VtxOffset)}: {VtxOffset}, {nameof(IdxOffset)}: {IdxOffset}, {nameof(TextureId)}: {TextureId}";
    }
}