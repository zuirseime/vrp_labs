using Blyskavitsya.Graphics;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace Blyskavitsya.Components;
public class Renderer : Component
{
    private VertexArray _vao = null!;
    private Buffer<Vector3> _vbo = null!;
    private Buffer<uint> _ebo = null!;
    private Shader _shader = null!;
    private Texture _texture = null!;
    private Material _material = null!;

    private Vector3[] _vertices = null!;
    private uint[] _indices = null!;
    private Vector2[] _texCoords = null!;

    public void LoadMeshData(Vector3[] vertices, uint[] indices, Vector2[] textureCoords)
    {
        _vertices = vertices;
        _indices = indices;
        _texCoords = textureCoords;
    }

    public void LoadShader(Shader shader) => _shader = shader;

    public void LoadTexture(string textureName)
    {
        _material.DiffuseMap.Texture = new Texture(textureName);
        _material.SpecularMap.Texture = new Texture(textureName);
    }

    protected override void Start()
    {
        _vao = new VertexArray();
        _vao.Bind();

        _vbo = new Buffer<Vector3>(BufferTarget.ArrayBuffer);
        _vbo.SetData(_vertices, BufferUsageHint.StaticDraw);

        Buffer<Vector2> uv = new(BufferTarget.ArrayBuffer);
        uv.SetData(_texCoords, BufferUsageHint.StaticDraw);

        _ebo = new Buffer<uint>(BufferTarget.ElementArrayBuffer);
        _ebo.SetData(_indices, BufferUsageHint.StaticDraw);

        _vao.LinkAttrib(_vbo, 0, 3);
        _vao.LinkAttrib(_vbo, 1, 2);
        _vao.LinkAttrib(uv, 2, 2);

        _material = new Material("default", Material.MaterialType.Diffuse);
        _material.SpecularMap.Color = Color4.Black;
    }

    protected override void Render()
    {
        Matrix4 model = Matrix4.Identity;
        Matrix4 view = Game.Instance.CurrentScene.MainCamera.GetViewMatrix();
        Matrix4 projection = Game.Instance.CurrentScene.MainCamera.GetProjectionMatrix();

        _shader.SetMatrix4(nameof(model), model);
        _shader.SetMatrix4(nameof(view), view);
        _shader.SetMatrix4(nameof(projection), projection);

        _shader.SetVector3("viewPos", Game.Instance.CurrentScene.MainCamera.transform.Position);

        _shader.Bind();
        _material.Apply(_shader);

        _vao.Bind();
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, nint.Zero);
        _vao.Unbind();
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _vao.Dispose();
                _vbo.Dispose();
                _ebo.Dispose();
                _material.Dispose();
                _shader.Dispose();
            }

            _disposed = true;
        }
    }
}
