using JetBrains.Annotations;

namespace imgui.NET;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
partial class ImFontAtlasCustomRect
{
    public bool IsPacked => X != 0xFFFF;

    /// <inheritdoc />
    public override string ToString()
    {
        return
            $"{nameof(Width)}: {Width}, {nameof(Height)}: {Height}, {nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(GlyphID)}: {GlyphID}, {nameof(GlyphAdvanceX)}: {GlyphAdvanceX}, {nameof(GlyphOffset)}: {GlyphOffset}";
    }
}