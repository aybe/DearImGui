using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using DearImGui;
using JetBrains.Annotations;

namespace DearImPlot;

/// <summary>
///     https://github.com/epezent/implot
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
partial class ImPlot
{
    /// <summary>
    ///     Indicates variable should deduced automatically.<br />
    ///     https://github.com/epezent/implot/blob/15e494b76a78b44ae2c1b76608ff9bc39a661409/implot.h#L65.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public const int IMPLOT_AUTO = -1;

    /// <summary>
    ///     Special color used to indicate that a color should be deduced automatically.
    ///     https://github.com/epezent/implot/blob/15e494b76a78b44ae2c1b76608ff9bc39a661409/implot.h#L67.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static readonly Vector4 IMPLOT_AUTO_COL = new(0, 0, 0, -1);

    static ImPlot()
    {
        var path = "implot";

        var image = SymbolResolver.LoadImage(ref path);

        if (image == IntPtr.Zero)
        {
            throw new DllNotFoundException(path);
        }
    }
}