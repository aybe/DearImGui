using JetBrains.Annotations;

namespace imgui.NET;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
partial class ImFont
{
    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{nameof(FontSize)}: {FontSize}, {nameof(Scale)}: {Scale}";
    }
}