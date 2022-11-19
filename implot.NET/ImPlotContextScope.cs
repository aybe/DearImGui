using JetBrains.Annotations;

namespace implot.NET;

/// <summary>
///     Scope for temporarily switching implot context.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public readonly struct ImPlotContextScope : IDisposable
{
    private readonly ImPlotContext Context;

#pragma warning disable CS1591
    public ImPlotContextScope(ImPlotContext context)
#pragma warning restore CS1591
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