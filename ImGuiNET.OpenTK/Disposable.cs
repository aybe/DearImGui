using JetBrains.Annotations;

namespace imgui.NET.OpenTK;

[PublicAPI]
public abstract class Disposable : IDisposable
{
    private bool IsDisposed { get; set; }

    #region IDisposable Members

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

    ~Disposable()
    {
        Dispose(false);
    }
}