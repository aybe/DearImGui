using JetBrains.Annotations;

namespace imgui.NET;

/// <summary>
///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L2711
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
partial class ImFontAtlasCustomRect
{
    /// <summary>
    ///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L2720
    /// </summary>
    public bool IsPacked => X != 0xFFFF;

    /// <inheritdoc />
    public override string ToString()
    {
        return
            $"{nameof(Width)}: {Width}, {nameof(Height)}: {Height}, {nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(GlyphID)}: {GlyphID}, {nameof(GlyphAdvanceX)}: {GlyphAdvanceX}, {nameof(GlyphOffset)}: {GlyphOffset}";
    }
}