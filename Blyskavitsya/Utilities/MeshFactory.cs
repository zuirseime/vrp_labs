
using OpenTK.Mathematics;

namespace Blyskavitsya.Utilities;
public class MeshFactory
{
    public static Mesh CreateCircle(int segments)
    {
        List<Vector3> vertices = [Vector3.Zero];
        List<int> indices = [];

        float angleStep = 2 * MathF.PI / segments;

        for (int i = 0; i <= segments + 1; i++)
        {
            float angle = i * angleStep;
            vertices.Add(new Vector3(MathF.Cos(angle), MathF.Sin(angle), 0f));
        }

        for (int i = 0; i <= segments; i++)
        {
            indices.Add(0);
            indices.Add(i);
            indices.Add(i + 1);
        }

        return BuildMesh(vertices, indices);
    }

    public static Mesh CreateSphere(int latitudeSegments, int longitudeSegments, float radius = 1f)
    {
        List<Vector3> vertices = [];
        List<int> indices = [];

        for (int lat = 0; lat <= latitudeSegments; lat++)
        {
            float theta = lat * MathF.PI / latitudeSegments;
            float sinTheta = MathF.Sin(theta);
            float cosTheta = MathF.Cos(theta);

            for (int lon = 0; lon <= longitudeSegments; lon++)
            {
                float phi = lon * 2 * MathF.PI / longitudeSegments;
                float sinPhi = MathF.Sin(phi);
                float cosPhi = MathF.Cos(phi);

                Vector3 position = new(radius * sinTheta * cosPhi, radius * cosTheta, radius * sinTheta * sinPhi);

                vertices.Add(position);
            }
        }

        for (int lat = 0; lat < latitudeSegments; lat++)
        {
            for (int lon = 0; lon < longitudeSegments; lon++)
            {
                int first = (lat * (longitudeSegments + 1)) + lon;
                int second = first + longitudeSegments + 1;

                indices.Add(first + 1);
                indices.Add(second);
                indices.Add(first);

                indices.Add(first + 1);
                indices.Add(second + 1);
                indices.Add(second);
            }
        }

        return BuildMesh(vertices, indices);
    }

    public static Mesh CreateCube(float size = 1)
    {
        float half = 1f / 2f;

        List<Vector3> vertices =
        [
            new(-half * size, -half * size,  half * size), 
            new( half * size, -half * size,  half * size), 
            new( half * size,  half * size,  half * size), 
            new(-half * size,  half * size,  half * size), 

            new(-half * size, -half * size, -half * size), 
            new( half * size, -half * size, -half * size), 
            new( half * size,  half * size, -half * size), 
            new(-half * size,  half * size, -half * size), 
        ];

        List<int> indices =
        [
            2, 3, 0,
            0, 1, 2,

            4, 7, 6,
            6, 5, 4,

            7, 4, 0,
            0, 3, 7,

            6, 2, 1,
            1, 5, 6,

            6, 7, 3,
            3, 2, 6,

            5, 1, 0,
            0, 4, 5,
        ];

        return BuildMesh(vertices, indices);
    }

    private static Mesh BuildMesh(List<Vector3> vertices, List<int> indices)
    {
        Mesh mesh = new() { Vertices = [.. vertices], Indices = [.. indices] };
        mesh.RecalculateNormals();
        return mesh;
    }
}
