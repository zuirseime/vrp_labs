using Blyskavitsya.Components;
using OpenTK.Mathematics;

namespace Blyskavitsya.Objects;

public enum LightType
{
    Directional,
    Point,
    Spot,
    Ambient
}

public class Light(string name = "Light", GameObject? parent = null) : GameObject(name, parent)
{
    protected Renderer? _renderer;

    public LightType Type { get; set; }

    public Color4 Color { get; set; } = Color4.White;
    public float Intensity { get; set; } = 1f;
    public float Radius { get; set; } = 10f;
    public float Cutoff { get; set; } = 12.5f;

    protected override void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    protected override void Update()
    {
        if (_renderer is not null && _renderer.Material is not null)
        {
            _renderer.Material.SetUniform(Graphics.UniformType.Color4, nameof(Color), Color);
            _renderer.Material.SetUniform(Graphics.UniformType.Float, nameof(Intensity), Intensity);
        } 
    }
}
