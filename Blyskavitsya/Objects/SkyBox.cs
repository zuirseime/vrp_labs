using Blyskavitsya.Components;
using Blyskavitsya.Utilities;
using OpenTK.Mathematics;

namespace Blyskavitsya.Objects;
public class SkyBox(string name = "Sky Box", GameObject? parent = null) : GameObject(name, parent)
{
    private Renderer? _renderer;

    protected override void Start()
    {
        _renderer = GetComponent<Renderer>();

        if (_renderer is null)
            return;

        _renderer.Mesh = MeshFactory.CreateCube();
        _renderer.CullBack = false;
        _renderer.Confirm();
    }

    protected override void Render()
    {
        var camera = Camera.MainCamera;

        if (_renderer is not null && camera is not null)
        {
            Matrix4 model = Transform.WorldMatrix;
            Matrix4 view = camera.GetViewMatrix().ClearTranslation();
            Matrix4 projection = camera.GetProjectionMatrix();

            if (_renderer.Material is not null)
            {
                _renderer.Material.SetUniform(Graphics.UniformType.Matrix4, nameof(model), model);
                _renderer.Material.SetUniform(Graphics.UniformType.Matrix4, nameof(view), view);
                _renderer.Material.SetUniform(Graphics.UniformType.Matrix4, nameof(projection), projection);
            }
        }

        Components.ForEach(c => c.OnRender());
    }
}
