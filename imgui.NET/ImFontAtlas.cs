using System.Numerics;
using JetBrains.Annotations;

namespace imgui.NET;

/// <summary>
///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L2749
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
partial class ImFontAtlas
{
    /// <summary>
    ///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L2833
    /// </summary>
    public unsafe Vector4[] TexUvLines
    {
        get
        {
            var value = new Vector4[64];

            Marshalling.Copy(ref value, ((__Internal*)__Instance)->TexUvLines);

            return value;
        }

        set
        {
            ArgumentNullException.ThrowIfNull(value);

            Marshalling.Copy(((__Internal*)__Instance)->TexUvLines, ref value);
        }
    }

    /// <summary>
    ///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L2772
    /// </summary>
    public bool IsBuilt()
    {
        return Fonts.Size > 0 && TexReady;
    }

    /// <summary>
    ///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L2773
    /// </summary>
    /// <param name="id"></param>
    public void SetTexID(IntPtr id)
    {
        TexID = id;
    }
}