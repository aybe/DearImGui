using JetBrains.Annotations;

namespace imgui.NET;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
partial class ImGuiKeyData
{
    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(Down)}: {Down}, {nameof(DownDuration)}: {DownDuration}, {nameof(DownDurationPrev)}: {DownDurationPrev}, {nameof(AnalogValue)}: {AnalogValue}";
    }
}