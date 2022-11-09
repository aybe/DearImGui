namespace imgui.NET;

partial class ImDrawCmd
{
    public unsafe IntPtr GetTexID()
    {
        // TODO keep synchronized with value in header as it may change
        return ((__Internal*)__Instance)->TextureId;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(ClipRect)}: {ClipRect}, {nameof(ElemCount)}: {ElemCount}, {nameof(VtxOffset)}: {VtxOffset}, {nameof(IdxOffset)}: {IdxOffset}, {nameof(TextureId)}: {TextureId}";
    }
}