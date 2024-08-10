using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace Blyskavitsya.Graphics;
internal class Buffer<T>(BufferTarget type) : IDisposable where T : struct
{
    private bool _disposed;

    internal int Handle { get; private set; } = GL.GenBuffer();
    internal BufferTarget Type { get; set; } = type;

    internal void Bind() => GL.BindBuffer(Type, Handle);

    internal void Unbind() => GL.BindBuffer(Type, 0);

    internal void SetData(T[] data, BufferUsageHint usage)
    {
        Bind();
        GL.BufferData(Type, data.Length * Marshal.SizeOf(typeof(T)), data, usage);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
                GL.DeleteBuffer(Handle);

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
