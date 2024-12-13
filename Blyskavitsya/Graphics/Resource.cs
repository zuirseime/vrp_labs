namespace Blyskavitsya.Graphics;
public abstract class Resource : IDisposable
{
    private bool _disposed;

    internal string Name { get; set; } = string.Empty;
    internal int Handle { get; private protected set; }

    protected virtual void Dispose(bool disposing) { }

    public void Dispose()
    {
        if (!_disposed)
        {
            Dispose(disposing: true);
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
