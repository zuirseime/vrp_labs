using OpenTK.Graphics.OpenGL;

namespace Blyskavitsya.Graphics;
internal class VertexArray : IDisposable
{
    private bool _disposed;

    internal int Handle { get; private set; } = GL.GenVertexArray();

    internal void Bind() => GL.BindVertexArray(Handle);
    internal void Unbind() => GL.BindVertexArray(0);

    internal void LinkAttrib<T>(Buffer<T> vbo, int layout, int size) where T : struct
    {
        Bind();
        vbo.Bind();
        GL.VertexAttribPointer(layout, size, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexAttribArray(layout);
        Unbind();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
                GL.DeleteVertexArray(Handle);

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
