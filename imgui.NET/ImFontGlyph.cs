using JetBrains.Annotations;

namespace imgui.NET;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
partial class ImFontGlyph
{
    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(Codepoint)}: {Codepoint}, {nameof(AdvanceX)}: {AdvanceX}";
    }
}