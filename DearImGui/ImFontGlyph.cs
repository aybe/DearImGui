using JetBrains.Annotations;

namespace DearImGui;

/// <summary>
///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L2684
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
partial class ImFontGlyph
{
    // ReSharper disable CommentTypo
    /// <summary>
    ///     Flag to indicate glyph is colored and should generally ignore tinting (make it usable with no shift on
    ///     little-endian as this is used in loops).<br />
    ///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L2686.
    /// </summary>
    // ReSharper restore CommentTypo
    public bool Colored
    {
        get
        {
            unsafe
            {
                return Marshalling.BitGet(((__Internal*)__Instance)->Colored, 0);
            }
        }
        set
        {
            unsafe
            {
                var instance = (__Internal*)__Instance;

                instance->Colored = Marshalling.BitSet(instance->Colored, 0, value);
            }
        }
    }

    /// <summary>
    ///     Flag to indicate glyph has no visible pixels (e.g. space). Allow early out when rendering.<br />
    ///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L2687.
    /// </summary>
    public bool Visible
    {
        get
        {
            unsafe
            {
                return Marshalling.BitGet(((__Internal*)__Instance)->Visible, 1);
            }
        }
        set
        {
            unsafe
            {
                var instance = (__Internal*)__Instance;

                instance->Visible = Marshalling.BitSet(instance->Visible, 1, value);
            }
        }
    }

    // ReSharper disable CommentTypo
    /// <summary>
    ///     0x0000..0x10FFFF.<br />
    ///     https://github.com/ocornut/imgui/blob/9aae45eb4a05a5a1f96be1ef37eb503a12ceb889/imgui.h#L2688.
    /// </summary>
    // ReSharper restore CommentTypo
    public uint Codepoint
    {
        get
        {
            unsafe
            {
                return ((__Internal*)__Instance)->Codepoint >> 2;
            }
        }

        set
        {
            unsafe
            {
                var instance = (__Internal*)__Instance;

                instance->Codepoint = (instance->Codepoint & 3) | (value << 2);
            }
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(Codepoint)}: {Codepoint}, {nameof(AdvanceX)}: {AdvanceX}";
    }
}