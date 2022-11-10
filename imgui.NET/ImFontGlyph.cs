namespace imgui.NET;

partial class ImFontGlyph
{
    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(Codepoint)}: {Codepoint}, {nameof(AdvanceX)}: {AdvanceX}";
    }
}