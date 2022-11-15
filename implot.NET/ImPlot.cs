using imgui.NET;
using JetBrains.Annotations;

namespace implot.NET;

[UsedImplicitly]
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