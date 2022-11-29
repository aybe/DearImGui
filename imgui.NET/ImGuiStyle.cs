using System.Numerics;
using JetBrains.Annotations;

namespace imgui.NET;

/// <summary>
///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L1840
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
partial class ImGuiStyle
{
    /// <summary>
    ///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L1882
    /// </summary>
    public Vector4[] Colors
    {
        get
        {
            var vectors = new Vector4[53];

            unsafe
            {
                Marshalling.Copy(ref vectors, ((__Internal*)__Instance)->Colors);
            }

            return vectors;
        }

        set
        {
            ArgumentNullException.ThrowIfNull(value);

            unsafe
            {
                Marshalling.Copy(((__Internal*)__Instance)->Colors, ref value);
            }
        }
    }
}