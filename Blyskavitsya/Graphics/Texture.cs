using OpenTK.Graphics.OpenGL;
using StbImageSharp;

namespace Blyskavitsya.Graphics;
public sealed class Texture : IDisposable
{
    private bool _disposed;

    internal int Handle { get; private set; }

    public Texture(string path)
    {
        Handle = GL.GenTexture();
        Bind();

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        StbImage.stbi_set_flip_vertically_on_load(1);
        ImageResult texture = ImageResult.FromStream(File.OpenRead(Path.Combine("public/textures", path)), ColorComponents.RedGreenBlueAlpha);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, texture.Width, texture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, texture.Data);

        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        Unbind();
    }

    internal void Bind(TextureUnit unit = TextureUnit.Texture0)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, Handle);
    }

    internal void Unbind() => GL.BindTexture(TextureTarget.Texture2D, 0);

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                GL.DeleteTexture(this.Handle);
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
