using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace Blyskavitsya.Graphics;
public class Buffer<T>(BufferTarget type) : IDisposable where T : struct
{
    private bool _disposed;

    internal int Handle { get; private set; } = GL.GenBuffer();

    internal void Bind() => GL.BindBuffer(type, Handle);

    internal void Unbind() => GL.BindBuffer(type, 0);

    internal void SetData(T[] data, BufferUsageHint usage)
    {
        Bind();
        GL.BufferData(type, data.Length * Marshal.SizeOf(typeof(T)), data, usage);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            GL.DeleteBuffer(Handle);
        }
    }

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
