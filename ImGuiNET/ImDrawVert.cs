namespace ImGuiNET;

partial class ImDrawVert // TODO struct
{
    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(Pos)}: {Pos:F3}, {nameof(Uv)}: {Uv:F2}, {nameof(Col)}: 0x{Col:X8}";
    }
}