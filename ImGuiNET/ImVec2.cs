using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace ImGuiNET;

[UsedImplicitly]
[SuppressMessage("ReSharper", "ArrangeTypeModifiers")]
partial struct ImVec2 // TODO this should be now made internal
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    internal partial struct __Internal
    {
        public override string ToString()
        {
            return $"{nameof(x)}: {x}, {nameof(y)}: {y}";
        }
    }
}