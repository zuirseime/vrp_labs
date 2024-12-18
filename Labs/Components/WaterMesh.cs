using Blyskavitsya.Components;
using Blyskavitsya.Graphics;
using Blyskavitsya;
using Labs.Components;
using OpenTK.Mathematics;

public class WaterMesh : Component
{
    public Renderer _renderer = null!;
    private MeshData _meshData = null!;
    private Dictionary<DrawMode, Texture> _textures = [];
    private DrawMode _drawMode = DrawMode.ColorMap;

    private int _width;
    private int _height;
    private float[,] _heightMap;

    public Biome[] Biomes { get; set; }

    protected override void Start()
    {
        _renderer = GetComponent<Renderer>()!;

        Biomes = [
            new Biome { Name = "Cold Deep Ocean", Color = new Color4(17, 22, 75, 255),
                        Height = HeightZone.Ocean, Temperature = TemperatureZone.Boreal },
            new Biome { Name = "Cool Deep Ocean", Color = new Color4(25, 45, 150, 255),
                        Height = HeightZone.Ocean, Temperature = TemperatureZone.Cool },
            new Biome { Name = "Warm Deep Ocean", Color = new Color4(40, 45, 155, 255),
                        Height = HeightZone.Ocean, Temperature = TemperatureZone.Tropical },

            new Biome { Name = "Cold Shallow Water", Color = new Color4(51, 66, 210, 255),
                        Height = HeightZone.Sea, Temperature = TemperatureZone.Boreal },
            new Biome { Name = "Cool Shallow Water", Color = new Color4(55, 120, 210, 255),
                        Height = HeightZone.Sea, Temperature = TemperatureZone.Cool },
            new Biome { Name = "Warm Shallow Water", Color = new Color4(62, 142, 210, 255),
                        Height = HeightZone.Sea, Temperature = TemperatureZone.Tropical },

            new Biome { Name = "River", Color = new Color4(62, 132, 201, 255),
                        Height = HeightZone.Upland }
        ];
    }

    public void DrawHeightMap(DrawMode mode, float[,] heightMap)
        => _textures.Add(mode, TexturePainter.DrawHeightMap(heightMap));

    public void DrawColorMap(float[,] heightMap, float[,] temperatureMap, float[,] humidityMap)
        => _textures.Add(DrawMode.ColorMap, TexturePainter.DrawColorMap(heightMap, temperatureMap, humidityMap, Biomes));

    public void CreateShape(float[,] heightMap, int width, int height)
    {
        _heightMap = heightMap;
        _width = width;
        _height = height;

        _meshData = new(width + 1, height + 1);

        GenerateMesh();
    }

    public void ConfirmMesh()
    {
        Mesh mesh = _meshData.GetMesh();
        _renderer.Mesh = mesh;
        _renderer.CullBack = false;
        _renderer.Opacity = 0.5f;
        _renderer.Material!.Texture = _textures[_drawMode];
        _renderer.Confirm();
    }

    private void GenerateMesh()
    {
        float topLeftX = _width / 2f;
        float topLeftZ = _height / 2f;

        for (int z = 0, i = 0, vertex = 0; z <= _height; z++)
        {
            for (int x = 0; x <= _width; x++, i++, vertex++)
            {
                _meshData.Vertices[i] = new Vector3(topLeftX - x, 25, topLeftZ - z);
                _meshData.UV[i] = new Vector2(x / (float)_width, z / (float)_height);

                if (x < _width - 1 && z < _height - 1)
                {
                    _meshData.AddTriangle(vertex + 1, vertex, vertex + _width + 1);
                    _meshData.AddTriangle(vertex + _width + 2, vertex + 1, vertex + _width + 1);
                }
            }
        }
    }

    protected override void Update()
    {
        _renderer.Material!.SetUniform(UniformType.Float, "time", Time.Total);
    }

    protected override void Dispose(bool disposing)
    {
        _meshData?.Dispose();
        _heightMap = new float[0, 0];
        Biomes = [];
    }
}