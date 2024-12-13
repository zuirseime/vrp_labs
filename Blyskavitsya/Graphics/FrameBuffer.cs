using OpenTK.Graphics.OpenGL;

namespace Blyskavitsya.Graphics;

internal enum FrameBufferDepth
{
    Texture, Buffer
}

public class FrameBuffer : IDisposable
{
    private bool _disposed;

    internal int Handle { get; private set; } = GL.GenFramebuffer();

    private Texture _colorTexture;
    private Texture _depthTexture;
    private int _depthBuffer;

    private int _width;
    private int _height;

    public Texture ColorTexture => _colorTexture;
    public Texture DepthTexture => _depthTexture;

    public FrameBuffer(int width, int height)
    {
        _width = width;
        _height = height;

        Create();
        _colorTexture = CreateColorTexture(width, height);
        _depthTexture = CreateDepthTexture(width, height);
        Unbind();
    }

    internal void Create()
    {
        Bind();
        GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
    }

    private Texture CreateColorTexture(int width, int height)
    {
        Texture texture = new(width, height);
        GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, texture.Handle, 0);
        return texture;
    }

    private Texture CreateDepthTexture(int width, int height)
    {
        Texture texture = new(width, height)
        {
            PixelInternalFormat = PixelInternalFormat.DepthComponent32,
            PixelFormat = PixelFormat.DepthComponent,
            FilterMode = TextureFilter.Nearest,
            WrapMode = TextureWrapMode.ClampToEdge
        };
        GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, texture.Handle, 0);
        return texture;
    }

    private int CreateDepthBuffer(int width, int height)
    {
        int buffer = GL.GenRenderbuffer();
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, buffer);
        GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent32, width, height);
        GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, buffer);
        return buffer;
    }

    public void Bind()
    {
        GL.BindTexture(TextureTarget.Texture2D, 0);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, Handle);
        GL.DrawBuffer(DrawBufferMode.None);
        GL.ReadBuffer(ReadBufferMode.None);
        GL.Viewport(0, 0, _depthTexture.Width, _depthTexture.Height);
    }

    public void BindColorTexture()
    {
        _colorTexture.Bind(1);
    }

    public void BindDepthTexture()
    {
        _depthTexture.Bind(1);
    }

    public void Unbind()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        GL.Viewport(0, 0, _depthTexture.Width, _depthTexture.Height);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                GL.DeleteFramebuffer(Handle);
                _colorTexture?.Dispose();
                _depthTexture?.Dispose();
                GL.DeleteRenderbuffer(_depthBuffer);
            }    

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
