using JetBrains.Annotations;

namespace ImGuiNET;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public partial class ImFont
{
    public override string ToString()
    {
        return $"{nameof(FontSize)}: {FontSize}, {nameof(Scale)}: {Scale}";
    }
}