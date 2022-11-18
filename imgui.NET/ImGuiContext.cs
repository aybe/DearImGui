﻿namespace imgui.NET;

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

    public static bool operator ==(ImGuiContext? left, ImGuiContext? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ImGuiContext? left, ImGuiContext? right)
    {
        return !Equals(left, right);
    }
}