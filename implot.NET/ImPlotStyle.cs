using System.Numerics;
using DearImGui;
using JetBrains.Annotations;

namespace implot.NET;

/// <summary>
///     https://github.com/epezent/implot/blob/15e494b76a78b44ae2c1b76608ff9bc39a661409/implot.h#L505
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
partial class ImPlotStyle
{
    /// <summary>
    ///     https://github.com/epezent/implot/blob/15e494b76a78b44ae2c1b76608ff9bc39a661409/implot.h#L536
    /// </summary>
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