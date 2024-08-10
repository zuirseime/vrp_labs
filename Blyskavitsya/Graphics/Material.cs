using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Blyskavitsya.Graphics;
public sealed class Material(string name, Material.MaterialType type) : IDisposable
{
    public enum MaterialType
    {
        Diffuse, Specular, Emission
    }

    public struct Map(Color4 color)
    {
        public Texture? Texture { get; set; }
        public Color4 Color { get; set; } = color;
    }

    private bool _disposed;

    public string Name { get; set; } = name;
    public MaterialType Type { get; set; } = type;
    public Map DiffuseMap = new(Color4.White);
    public Map SpecularMap = new(Color4.White);
    public Map EmissionMap = new(Color4.Black);
    public float Shininess { get; set; } = 32f;

    public void Apply(Shader shader)
    {
        shader.SetVector4("material.diffuse.color", DiffuseMap.Color);
        shader.SetVector4("material.specular.color", SpecularMap.Color);
        shader.SetVector4("material.emission.color", EmissionMap.Color);
        shader.SetFloat("material.shininess", Shininess);

        if (DiffuseMap.Texture != null)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            DiffuseMap.Texture.Bind();
            shader.SetInt("material.diffuse.tex", 0);
        }

        if (SpecularMap.Texture != null)
        {
            GL.ActiveTexture(TextureUnit.Texture1);
            SpecularMap.Texture.Bind(TextureUnit.Texture1);
            shader.SetInt("material.specular.tex", 1);
        }

        if (EmissionMap.Texture != null)
        {
            GL.ActiveTexture(TextureUnit.Texture2);
            EmissionMap.Texture.Bind(TextureUnit.Texture2);
            shader.SetInt("material.emission.tex", 2);
        }
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                DiffuseMap.Texture?.Dispose();
                SpecularMap.Texture?.Dispose();
                EmissionMap.Texture?.Dispose();
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
