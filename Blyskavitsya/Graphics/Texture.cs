using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using StbImageSharp;

namespace Blyskavitsya.Graphics;

public enum TextureFilter
{
    Nearest = 9728,
    Linear = 9729,
}

public sealed class Texture : Resource
{
    private byte[]? _data;

    public int Width { get; private set; }
    public int Height { get; private set; }

    public TextureWrapMode WrapMode { get; set; } = TextureWrapMode.Repeat;
    public TextureFilter FilterMode { get; set; } = TextureFilter.Linear;
    internal PixelInternalFormat PixelInternalFormat { get; set; } = PixelInternalFormat.Rgba;
    internal PixelFormat PixelFormat { get; set; } = PixelFormat.Rgba;

    public Texture(string path)
    {
        PrepareTexture();

        StbImage.stbi_set_flip_vertically_on_load(1);

        ImageResult texture = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);

        Width = texture.Width;
        Height = texture.Height;
        _data = texture.Data;
    }

    public Texture(int width, int height)
    {
        PrepareTexture();

        Width = width;
        Height = height;
    }

    private void PrepareTexture()
    {
        Handle = GL.GenTexture();
        SetTextureParameters();
    }

    public void SetData(Color4[] data)
    {
        if (data.Length != Width * Height)
            throw new ArgumentException("The size of the Color4[] array does not match Width and Height.");

        _data = new byte[data.Length * 4];

        for (int i = 0; i < data.Length; i++)
        {
            _data[i * 4 + 0] = (byte)Math.Clamp((int)(data[i].R * 255.0f), 0, 255);
            _data[i * 4 + 1] = (byte)Math.Clamp((int)(data[i].G * 255.0f), 0, 255);
            _data[i * 4 + 2] = (byte)Math.Clamp((int)(data[i].B * 255.0f), 0, 255);
            _data[i * 4 + 3] = (byte)Math.Clamp((int)(data[i].A * 255.0f), 0, 255);
        }
    }

    public void Apply()
    {
        if (_data is null)
            throw new ArgumentException("Unable to apply a texture because the texture data is empty.");

        GL.TexImage2D(TextureTarget.Texture2D,
                      0, PixelInternalFormat,
                      Width, Height, 0, PixelFormat,
                      PixelType.UnsignedByte, _data
        );

        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
    }

    private void SetTextureParameters()
    {
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)WrapMode);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)WrapMode);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)FilterMode);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)FilterMode);
    }

    internal void Bind(int unit = 0)
    {
        GL.ActiveTexture(TextureUnit.Texture0 + unit);
        GL.BindTexture(TextureTarget.Texture2D, Handle);
    }

    internal void Unbind() => GL.BindTexture(TextureTarget.Texture2D, 0);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Unbind();
            GL.DeleteTexture(Handle);
        }
    }
}
