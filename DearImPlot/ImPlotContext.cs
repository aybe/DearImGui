namespace DearImPlot;

/// <summary>
///     https://github.com/epezent/implot/blob/15e494b76a78b44ae2c1b76608ff9bc39a661409/implot.h#L76
/// </summary>
partial class ImPlotContext : IEquatable<ImPlotContext>
{
    #region IEquatable<ImPlotContext> Members

    /// <inheritdoc />
    public bool Equals(ImPlotContext? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return __Instance.Equals(other.__Instance);
    }

    #endregion

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((ImPlotContext)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return __Instance.GetHashCode();
    }

#pragma warning disable CS1591
    public static bool operator ==(ImPlotContext? left, ImPlotContext? right)
#pragma warning restore CS1591
    {
        return Equals(left, right);
    }

#pragma warning disable CS1591
    public static bool operator !=(ImPlotContext? left, ImPlotContext? right)
#pragma warning restore CS1591
    {
        return !Equals(left, right);
    }
}