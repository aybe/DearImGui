using DearImGui;
using JetBrains.Annotations;

namespace implot.NET;

/// <summary>
///     https://github.com/epezent/implot
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
partial class ImPlot
{
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