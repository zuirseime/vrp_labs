using OpenTK.Graphics.OpenGL;

namespace Blyskavitsya.Graphics;
public class VertexArray : IDisposable
{
    private bool _disposed;

    internal int Handle { get; private set; } = GL.GenVertexArray();

    internal void Bind() => GL.BindVertexArray(Handle);
    internal void Unbind() => GL.BindVertexArray(0);

    internal void LinkAttrib<T>(Buffer<T> vbo, int layout, int size) where T : struct
    {
        vbo.Bind();
        GL.VertexAttribPointer(layout, size, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexAttribArray(layout);
        vbo.Unbind();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            GL.DeleteVertexArray(Handle);
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
