using JetBrains.Annotations;

namespace imgui.NET;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
partial class ImGuiContext : IEquatable<ImGuiContext>
{
    #region IEquatable<ImGuiContext> Members

    /// <inheritdoc />
    public bool Equals(ImGuiContext? other)
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

        return Equals((ImGuiContext)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return __Instance.GetHashCode();
    }

#pragma warning disable CS1591
    public static bool operator ==(ImGuiContext? left, ImGuiContext? right)
#pragma warning restore CS1591
    {
        return Equals(left, right);
    }

#pragma warning disable CS1591
    public static bool operator !=(ImGuiContext? left, ImGuiContext? right)
#pragma warning restore CS1591
    {
        return !Equals(left, right);
    }
}