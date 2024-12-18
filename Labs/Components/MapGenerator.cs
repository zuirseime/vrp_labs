using Blyskavitsya;
using OpenTK.Mathematics;

namespace Labs.Components;
internal class MapGenerator : Component
{
    public int Width { get; set; } = (int)MathF.Pow(2, 10);
    public int Height { get; set; } = (int)MathF.Pow(2, 10);

    public float NoiseScale { get; set; } = 2500f;
    public int Octaves { get; set; } = 15;
    public float Persistance { get; set; } = 0.35f;
    public float Lacunarity { get; set; } = 3f;
    public int Seed { get; set; } = 157;
    public Vector2 Offset { get; set; } = Vector2.Zero;

    private TerrainMesh? _terrain;
    private WaterMesh? _water;

    protected override async void Start()
    {
        var heightTask = GetMap(Width, Height, Seed, NoiseScale, Octaves, Persistance, Lacunarity, Offset);
        var temperatureTask = GetMap(Width, Height, Seed, NoiseScale, 5, Persistance / 1.5f, Lacunarity, Offset);
        var humidityTask = GetMap(Width, Height, Seed, NoiseScale / 2f, 6, Persistance / 1.5f, Lacunarity, Offset);
        Task.WaitAll(heightTask, temperatureTask, humidityTask);

        float[,] heightMap = await heightTask;
        float[,] temperatureMap = await temperatureTask;
        float[,] humidityMap = await humidityTask;

        _terrain = GameObject.FindObjectOfType<TerrainMesh>();
        _water = GameObject.FindObjectOfType<WaterMesh>();
        if (_terrain is null || _water is null)
            return;

        _terrain.CreateShape(heightMap, Width, Height);
        _terrain.DrawColorMap(heightMap, temperatureMap, humidityMap);

        _water.CreateShape(heightMap, Width, Height);
        _water.DrawColorMap(heightMap, temperatureMap, humidityMap);

        _terrain.ConfirmMesh();
        _water.ConfirmMesh();

        _terrain.Dispose();
        _water.Dispose();
    }

    private async Task<float[,]> GetMap(int width, int height, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
        => await Task.Run(() => NoiseGenerator.GenerateMap(width + 1, height + 1, seed, scale, octaves, persistance, lacunarity, offset));
}