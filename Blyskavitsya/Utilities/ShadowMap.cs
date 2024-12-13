using OpenTK.Graphics.OpenGL;

namespace Blyskavitsya.Utilities;
public class ShadowMap : IDisposable
{
    public int Width { get; }
    public int Height { get; }
    public int DepthTexture { get; }
    public int FBO { get; }

    public ShadowMap(int width, int height)
    {
        Width = width;
        Height = height;

        FBO = GL.GenFramebuffer();
        DepthTexture = GL.GenTexture();

        GL.BindTexture(TextureTarget.Texture2D, DepthTexture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, width, height, 0,
                      PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);
        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                                TextureTarget.Texture2D, DepthTexture, 0);

        GL.DrawBuffer(DrawBufferMode.None);
        GL.ReadBuffer(ReadBufferMode.None);

        if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            throw new Exception("Framebuffer not ready");

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public void Bind()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);
        GL.Viewport(0, 0, Width, Height);
        GL.Clear(ClearBufferMask.DepthBufferBit);
    }

    public void Unbind(int windowWidth, int windowHeight)
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        GL.Viewport(0, 0, windowWidth, windowHeight);
    }

    public void Dispose()
    {
        GL.DeleteTexture(DepthTexture);
        GL.DeleteFramebuffer(FBO);
    }
}
