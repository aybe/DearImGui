using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using JetBrains.Annotations;

namespace imgui.NET;

/// <summary>
///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L1905
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
partial class ImGuiIO
{
    /// <summary>
    ///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L2038
    /// </summary>
    public Vector2[] MouseClickedPos
    {
        get
        {
            var value = new Vector2[5];

            unsafe
            {
                Marshalling.Copy(ref value, ((__Internal*)__Instance)->MouseClickedPos);
            }

            return value;
        }

        set
        {
            ArgumentNullException.ThrowIfNull(value);

            unsafe
            {
                Marshalling.Copy(((__Internal*)__Instance)->MouseClickedPos, ref value);
            }
        }
    }
}