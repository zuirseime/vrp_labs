using Blyskavitsya;
using OpenTK.Mathematics;

namespace Labs.Components;
public class MeshData : IDisposable
{
    private bool _disposed;
    private int _triangle = 0;

    public Vector3[] Vertices { get; set; } = null!;
    public int[] Triangles { get; set; } = null!;
    public float[] AmbientOcclusion { get; set; } = null!;
    public Vector2[] UV { get; set; } = null!;
    public Color4[] ColorMap { get; set; }

    public MeshData(int width, int height)
    {
        Vertices = new Vector3[width * height];
        Triangles = new int[(width - 1) * (height - 1) * 6];
        UV = new Vector2[Vertices.Length];
        AmbientOcclusion = new float[Vertices.Length];
        ColorMap = new Color4[Vertices.Length];
    }

    public void AddTriangle(int v1, int v2, int v3)
    {
        Triangles[_triangle++] = v1;
        Triangles[_triangle++] = v2;
        Triangles[_triangle++] = v3;
    }

    public Mesh GetMesh()
    {
        Mesh mesh = new()
        {
            Vertices = Vertices,
            Indices = Triangles,
            UV = UV,
            AmbientOcclusion = AmbientOcclusion,
        };
        mesh.RecalculateNormals();
        return mesh;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            Dispose(true);
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            Vertices = [];
            Triangles = [];
            AmbientOcclusion = [];
            UV = [];
        }
    }
}