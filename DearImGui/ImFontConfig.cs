using JetBrains.Annotations;

namespace DearImGui;

/// <summary>
///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L2655
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
unsafe partial class ImFontConfig
{
    /// <summary>
    ///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L2676
    /// </summary>
    public string Name => Marshalling.ReadString(((__Internal*)__Instance)->Name);
}