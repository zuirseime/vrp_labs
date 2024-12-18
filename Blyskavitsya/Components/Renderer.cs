using Blyskavitsya.Graphics;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace Blyskavitsya.Components;
public class Renderer : Component
{
    protected VertexArray _vao = null!;
    protected Buffer<Vector3> _vbo = null!;
    protected Buffer<int> _ebo = null!;

    private Buffer<Vector3> _normal = null!;
    private Buffer<float>? _ao;

    public Material? Material { get; set; }
    public Mesh? Mesh { get; set; }

    public bool CullBack { get; set; } = true;
    public float Opacity { get; set; } = 1.0f;

    protected virtual void SetBuffers()
    {
        if (Mesh is null) return;

        if (Mesh.Normals is not null)
        {
            _normal = new(BufferTarget.ArrayBuffer);
            _normal.SetData(Mesh.Normals, BufferUsageHint.StaticDraw);
        }

        if (Mesh.AmbientOcclusion is not null)
        {
            _ao = new(BufferTarget.ArrayBuffer);
            _ao.SetData(Mesh.AmbientOcclusion, BufferUsageHint.StaticDraw);
        }
    }

    protected virtual void LinkBuffers()
    {
        if (_normal is not null)
            _vao.LinkAttrib(_normal, 2, 3);

        if (_ao is not null)
            _vao.LinkAttrib(_ao, 3, 1);
    }

    protected virtual void UnbindBuffers()
    {
        _normal?.Unbind();
        _ao?.Unbind();

        _normal?.Dispose();
        _ao?.Dispose();
    }

    public virtual void Confirm()
    {
        if (Mesh is null || Material is null)
            return;

        _vao = new VertexArray();
        _vao.Bind();

        _vbo = new Buffer<Vector3>(BufferTarget.ArrayBuffer);
        _vbo.SetData(Mesh.Vertices, BufferUsageHint.StaticDraw);

        _ebo = new Buffer<int>(BufferTarget.ElementArrayBuffer);
        _ebo.SetData(Mesh.Indices, BufferUsageHint.StaticDraw);

        Buffer<Vector2> uv = new(BufferTarget.ArrayBuffer);
        if (Mesh.UV is not null)
            uv.SetData(Mesh.UV, BufferUsageHint.StaticDraw);

        SetBuffers();

        _vao.LinkAttrib(_vbo, 0, 3);
        if (Mesh.UV is not null)
            _vao.LinkAttrib(uv, 1, 2);
        LinkBuffers();

        _vao.Unbind();

        _vbo.Unbind();
        _ebo.Unbind();
        uv.Unbind();

        _vbo.Dispose();
        _ebo.Dispose();
        uv.Dispose();

        UnbindBuffers();
    }

    protected override void Render()
    {
        //var distance = Vector3.Distance(Transform.Position, Camera.MainCamera.Transform.Position);
        if (Material is null || !GameObject.Visible) // distance > RenderDistance
            return;

        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        if (CullBack)
        {
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
        }

        Material.SetUniform(UniformType.Float, nameof(Opacity), Opacity);

        Material.Bind();
        Material.Apply();

        if (Mesh is not null)
        {
            _vao.Bind();
            GL.DrawElements(PrimitiveType.Triangles, Mesh.Indices.Length, DrawElementsType.UnsignedInt, nint.Zero);
            _vao.Unbind();
        }
        
        Material.Unbind();

        if (CullBack)
            GL.Disable(EnableCap.CullFace);

        GL.Disable(EnableCap.Blend);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _ = this.GameObject;
                _vao?.Dispose();
                _vbo?.Dispose();
                _ebo?.Dispose();
                _normal?.Dispose();
                _ao?.Dispose();
                Material?.Dispose();
                Mesh?.Dispose();
            }

            _disposed = true;
        }
    }
}
