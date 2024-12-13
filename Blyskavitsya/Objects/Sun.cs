using Blyskavitsya.Utilities;
using OpenTK.Mathematics;
using System.Xml.Linq;

namespace Blyskavitsya.Objects;
public class Sun : Light
{
    public Sun(string name = "Sun", GameObject? parent = null) : base(name, parent)
    {
        Type = LightType.Point;
        Color = new Color4(1f, 0.8f, 0.5f, 1f);
        Intensity = 100000f;
    }

    protected override void Start()
    {
        base.Start();

        Transform.Scale = Vector3.One * 100f;

        if (_renderer is null || _renderer.Material is null)
            return;

        _renderer.Mesh = MeshFactory.CreateCircle(32);
        _renderer.CullBack = false;
        _renderer.Confirm();
    }

    protected override void Update()
    {
        LookAt(Vector3.Zero);
    }

    protected override void Render()
    {
        var camera = Camera.MainCamera;

        if (_renderer is not null && camera is not null)
        {
            Matrix4 model = Transform.WorldMatrix;
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjectionMatrix();

            if (_renderer.Material is not null)
            {
                _renderer.Material.SetUniform(Graphics.UniformType.Vector3, nameof(Transform.Position), Transform.Position);
                _renderer.Material.SetUniform(Graphics.UniformType.Vector3, nameof(Transform.Scale), Transform.Scale);
                _renderer.Material.SetUniform(Graphics.UniformType.Float, nameof(Intensity), Intensity);
                _renderer.Material.SetUniform(Graphics.UniformType.Float, nameof(Radius), Radius);
                _renderer.Material.SetUniform(Graphics.UniformType.Float, nameof(Cutoff), Cutoff);
                _renderer.Material.SetUniform(Graphics.UniformType.Matrix4, nameof(model), model);
                _renderer.Material.SetUniform(Graphics.UniformType.Matrix4, nameof(view), view);
                _renderer.Material.SetUniform(Graphics.UniformType.Matrix4, nameof(projection), projection);
            }
        }

        Components.ForEach(c => c.OnRender());
    }
}
