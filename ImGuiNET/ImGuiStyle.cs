using System.Numerics;

namespace ImGuiNET;

partial class ImGuiStyle
{
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