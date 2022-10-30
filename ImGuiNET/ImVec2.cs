using JetBrains.Annotations;

namespace ImGuiNET;

[UsedImplicitly]
public partial struct ImVec2
{
    public override string ToString()
    {
        return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}";
    }
}