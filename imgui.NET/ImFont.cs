using JetBrains.Annotations;

namespace imgui.NET;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
partial class ImFont
{
    public string DebugName => ConfigData != null ? new string(ConfigData.Name).TrimEnd('\0') : "<unknown>";

    public bool IsLoaded => ContainerAtlas != null;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(FontSize)}: {FontSize}, {nameof(Scale)}: {Scale}";
    }
}