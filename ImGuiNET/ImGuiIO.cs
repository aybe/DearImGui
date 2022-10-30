using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace ImGuiNET;

[SuppressMessage("ReSharper", "InconsistentNaming")]
partial class ImGuiIO
{
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