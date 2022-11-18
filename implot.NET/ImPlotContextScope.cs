namespace implot.NET;

public readonly struct ImPlotContextScope : IDisposable
{
    private readonly ImPlotContext Context;

    public ImPlotContextScope(ImPlotContext context)
    {
        Context = ImPlot.GetCurrentContext();

        ImPlot.SetCurrentContext(context);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        ImPlot.SetCurrentContext(Context);
    }
}