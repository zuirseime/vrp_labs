using Blyskavitsya.Objects;
using OpenTK.Mathematics;
using System.Reflection;
using System.Text.Json;

namespace Blyskavitsya.Graphics;

public enum UniformType
{
    Int, Float, Vector3, Vector4, Color4, Matrix4
}

public enum MaterialType
{
    Lit, Unlit
}

public class Uniform(UniformType type, object value)
{
    public UniformType Type { get; set; } = type;
    public object Value { get; set; } = value;
}

public class Material(Shader shader, MaterialType type = MaterialType.Lit) : Resource
{
    private readonly Dictionary<string, Uniform> _uniforms = [];
    private Shader _shader = shader;

    public Texture? Texture { get; set; }
    public Color4 Color { get; set; } = Color4.White;
    public MaterialType Type { get; set; } = type;

    public void SetUniform(UniformType type, string name, object value)
    {
        if (_uniforms.TryGetValue(name, out Uniform? uniform))
            uniform.Value = value;
        else _uniforms.Add(name, new Uniform(type, value));
    }

    internal void Bind()
    {
        _shader.Bind();
        Texture?.Bind();
    }
    internal void Unbind()
    {
        Texture?.Unbind();
        _shader.Unbind();
    }

    internal void Apply()
    {
        Texture?.Apply();
        _shader.SetInt(nameof(Texture), 0);

        var camera = Camera.MainCamera;
        _shader.SetColor4(nameof(Color), Color);
        _shader.SetVector3(nameof(Camera), camera.Transform.Position);
        _shader.SetFloat("renderDistance", RenderDistance);

        if (Type == MaterialType.Lit)
        {
            List<Light?> lights = [];
            lights.Add(GameObject.FindObjectOfType<Sun>());
            lights.Add(GameObject.FindObjectOfType<Moon>());
            lights.AddRange(GameObject.FindObjectsOfType<Light>().Where(l 
                => Vector3.Distance(l.Transform.Position, camera.Transform.Position) <= RenderDistance));
            
            for (int i = 0; i < lights.Count; i++)
            {
                if (lights[i] is not null)
                {
                    _shader.SetInt($"{nameof(lights)}[{i}].{nameof(Light.Type)}", (int)lights[i]!.Type);
                    _shader.SetVector3($"{nameof(lights)}[{i}].{nameof(Light.Transform.Position)}", lights[i]!.Transform.Position);
                    _shader.SetVector3($"{nameof(lights)}[{i}].Direction", lights[i]!.Transform.Forward);
                    _shader.SetColor4($"{nameof(lights)}[{i}].{nameof(Light.Color)}", lights[i]!.Color);
                    _shader.SetFloat($"{nameof(lights)}[{i}].{nameof(Light.Intensity)}", lights[i]!.Intensity);
                }
            }

            _shader.SetInt("lightCount", lights.Count);
        }

        foreach (var kvp in _uniforms)
        {
            var method = typeof(Shader).GetMethod($"Set{kvp.Value.Type}", BindingFlags.Instance | BindingFlags.Public);
            method?.Invoke(_shader, [kvp.Key, kvp.Value.Value]);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing)
            return;

        _uniforms.Clear();
        _shader?.Dispose();
        Texture?.Dispose();
    }
}
