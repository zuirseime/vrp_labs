using OpenTK.Mathematics;

namespace Blyskavitsya.Utilities;
public class MeshMerger
{
    public static Mesh Merge(IEnumerable<(Mesh mesh, Vector3 position, Vector3 scale)> meshData)
    {
        List<Vector3> mergedVertices = [];
        List<int> mergedIndices = [];

        int vertexOffset = 0;

        foreach (var (mesh, position, scale) in meshData)
        {
            foreach (var vertex in mesh.Vertices)
            {
                var scaledVertex = Vector3.Multiply(vertex, scale);
                mergedVertices.Add(scaledVertex + position);
            }

            foreach (var index in mesh.Indices)
                mergedIndices.Add(index + vertexOffset);

            vertexOffset += mesh.Vertices.Length;
        }

        var mergedMesh = new Mesh
        {
            Vertices = [.. mergedVertices],
            Indices = [.. mergedIndices],
        };

        mergedMesh.RecalculateNormals();

        return mergedMesh;
    }
}
