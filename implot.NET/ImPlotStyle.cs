using System.Numerics;
using imgui.NET;
using JetBrains.Annotations;

namespace implot.NET;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
partial class ImPlotStyle
{
    public Vector4[] Colors
    {
        get
        {
            var vectors = new Vector4[21];

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
                Marshalling.Copy(((ImGuiStyle.__Internal*)__Instance)->Colors, ref value);
            }
        }
    }
}