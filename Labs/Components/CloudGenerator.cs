using Blyskavitsya.Components;
using Blyskavitsya.Graphics;
using Blyskavitsya.Utilities;
using Blyskavitsya;
using OpenTK.Mathematics;

namespace Labs.Components;
public class CloudGenerator : Component
{
    public float Threshold { get; set; } = 0.75f;
    public int Step { get; set; } = 1;

    public int Width { get; set; } = 200;
    public int Height { get; set; } = 200;
    public Material Material { get; set; } = null!;

    protected override void Start()
    {
        var heightMap = NoiseGenerator.GenerateMap(Width, Height, 0, 100, 2, 0.6f, 10, Vector2.Zero);

        float halfWidth = Width / 2f;
        float halfHeight = Height / 2f;

        bool[,] visited = new bool[Width, Height];

        for (int z = 0; z < Height; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (visited[x, z] || heightMap[x, z] < Threshold)
                    continue;

                List<(Vector3 position, Vector3 scale)> cluster = FindCluster(x, z, heightMap, visited, halfWidth, halfHeight);

                Vector3 avg = new(
                    cluster.Average(c => c.position.X),
                    cluster.Average(c => c.position.Y),
                    cluster.Average(c => c.position.Z)
                );

                GameObject cloud = new($"Cloud [{avg.X - halfWidth}; {avg.Z - halfHeight}]", GameObject);
                cloud.Transform.Position = new Vector3(avg.X - halfWidth, 0, avg.Z - halfHeight);

                Renderer renderer = cloud.AddComponent<Renderer>();

                List<(Mesh, Vector3, Vector3)> meshes = [];
                foreach (var (position, scale) in cluster)
                {
                    meshes.Add((MeshFactory.CreateCube(), position, scale));
                }
                Mesh mesh = MeshMerger.Merge(meshes);

                Material.Color = new Color4(1, 1, 1, 0.75f);

                renderer.Mesh = mesh;
                renderer.Material = Material;
                renderer.CullBack = false;
                renderer.Confirm();
            }
        }
    }

    private List<(Vector3 position, Vector3 scale)> FindCluster(int startX, int startZ, float[,] heightMap, bool[,] visited, float halfWidth, float halfHeight)
    {
        List<(Vector3 position, Vector3 scale)> cluster = [];
        Queue<(int x, int z)> queue = [];
        queue.Enqueue((startX, startZ));
        visited[startX, startZ] = true;

        while (queue.Count > 0)
        {
            var (x, z) = queue.Dequeue();
            float height = heightMap[x, z];

            Vector3 position = new(x - halfWidth, 20 + height / 2, z - halfHeight);
            Vector3 scale = new(1f, 1f, 1f);
            cluster.Add((position, scale));

            foreach (var (nx, nz) in GetNeighbors(x, z))
            {
                if (!visited[nx, nz] && heightMap[nx, nz] >= Threshold)
                {
                    visited[nx, nz] = true;
                    queue.Enqueue((nx, nz));
                }
            }
        }

        return cluster;
    }

    private IEnumerable<(int, int)> GetNeighbors(int x, int z)
    {
        if (x > 0)
            yield return (x - 1, z);
        if (x < Width - 1)
            yield return (x + 1, z);
        if (z > 0)
            yield return (x, z - 1);
        if (z < Height - 1)
            yield return (x, z + 1);
    }
}
