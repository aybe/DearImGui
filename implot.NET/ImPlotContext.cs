namespace implot.NET;

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

    public static bool operator ==(ImPlotContext? left, ImPlotContext? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ImPlotContext? left, ImPlotContext? right)
    {
        return !Equals(left, right);
    }
}