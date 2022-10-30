using JetBrains.Annotations;

namespace ImGuiNET;

[UsedImplicitly]
public partial struct ImVec4
{
    public override string ToString()
    {
        return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}, {nameof(W)}: {W}";
    }
}