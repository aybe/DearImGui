#pragma warning disable CS1591
using JetBrains.Annotations;

namespace DearImGui.OpenTK.Extensions;

/// <summary>
///     Base class for a disposable object.
/// </summary>
[PublicAPI]
public abstract class Disposable : IDisposable
{
    private bool IsDisposed { get; set; }

    #region IDisposable Members

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion

    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }

        DisposeNative();

        if (disposing)
        {
            DisposeManaged();
        }

        IsDisposed = true;
    }

    protected virtual void DisposeManaged()
    {
    }

    protected virtual void DisposeNative()
    {
    }

    /// <inheritdoc />
    ~Disposable()
    {
        Dispose(false);
    }
}