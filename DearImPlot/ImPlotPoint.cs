using JetBrains.Annotations;

namespace DearImPlot;

/// <summary>
///     https://github.com/epezent/implot/blob/15e494b76a78b44ae2c1b76608ff9bc39a661409/implot.h#L467
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
partial struct ImPlotPoint
{
#pragma warning disable CS1591
    public double this[int index]
#pragma warning restore CS1591
    {
        get
        {
            return index switch
            {
                0 => X,
                1 => Y,
                _ => throw new IndexOutOfRangeException()
            };
        }
        set
        {
            switch (index)
            {
                case 0:
                    X = value;
                    break;
                case 1:
                    Y = value;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }
}