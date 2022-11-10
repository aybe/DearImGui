using JetBrains.Annotations;

namespace imgui.NET;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
partial class ImFont
{
    public string DebugName => ConfigData?.Name ?? "<unknown>";

    public bool IsLoaded => ContainerAtlas != null;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(FontSize)}: {FontSize}, {nameof(Scale)}: {Scale}";
    }
}