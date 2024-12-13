using OpenTK.Mathematics;

namespace Blyskavitsya;
public class Mesh : IDisposable
{
    private bool _disposed;

    public Vector3[] Vertices { get; set; } = null!;
    public int[] Indices { get; set; } = null!;
    public Vector2[] UV { get; set; } = null!;
    public float[] AmbientOcclusion { get; set; } = null!;
    internal Vector3[] Normals { get; set; } = null!;

    public void RecalculateNormals()
    {
        Normals = new Vector3[Vertices.Length];

        for (int i = 0; i < Indices.Length; i += 3)
        {
            int i1 = Indices[i];
            int i2 = Indices[i + 1];
            int i3 = Indices[i + 2];

            Vector3 v1 = Vertices[i2] - Vertices[i1];
            Vector3 v2 = Vertices[i3] - Vertices[i1];
            Vector3 normal = Vector3.Cross(v1, v2).Normalized();

            Normals[i1] += normal;
            Normals[i2] += normal;
            Normals[i3] += normal;
        }

        for (int i = 0; i < Normals.Length; i++)
            Normals[i] = Normals[i].Normalized();
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
            Indices = [];
            UV = [];
            AmbientOcclusion = [];
            Normals = [];
        }
    }
}
