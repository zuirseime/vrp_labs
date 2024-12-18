using Blyskavitsya.Components;
using Blyskavitsya.Graphics;
using Blyskavitsya;
using OpenTK.Mathematics;

namespace Labs.Components;
public class TerrainMesh : Component
{
    public Renderer _renderer = null!;
    private MeshData _meshData = null!;
    private Dictionary<DrawMode, Texture> _textures = [];
    private DrawMode _drawMode = DrawMode.ColorMap;

    private int _width;
    private int _height;
    private float[,] _heightMap = null!;

    public Biome[] Biomes { get; set; }

    public float HeightMultiplier { get; set; } = 150f;

    protected override void Start()
    {
        _renderer = GetComponent<Renderer>()!;

        Biomes = [
            new Biome { Name = "Cold Deep Ocean", Color = new Color4(42, 35, 53, 255),
                        Height = HeightZone.Ocean, Temperature = TemperatureZone.Boreal },
            new Biome { Name = "Cool Deep Ocean", Color = new Color4(95, 82, 87, 255),
                        Height = HeightZone.Ocean, Temperature = TemperatureZone.Cool },
            new Biome { Name = "Warm Deep Ocean", Color = new Color4(110, 96, 88, 255),
                        Height = HeightZone.Ocean, Temperature = TemperatureZone.Tropical },

            new Biome { Name = "Cold Shallow Water", Color = new Color4(85, 77, 80, 255),
                        Height = HeightZone.Sea, Temperature = TemperatureZone.Boreal },
            new Biome { Name = "Cool Shallow Water", Color = new Color4(90, 70, 80, 255),
                        Height = HeightZone.Sea, Temperature = TemperatureZone.Cool },
            new Biome { Name = "Warm Shallow Water", Color = new Color4(225, 180, 141, 255),
                        Height = HeightZone.Sea, Temperature = TemperatureZone.Tropical },

            new Biome { Name = "Сoast", Color = new Color4(90, 173, 89, 255),
                        Height = HeightZone.Coast, Temperature = TemperatureZone.Cool },
            new Biome { Name = "Beach", Color = new Color4(255, 225, 141, 255),
                        Height = HeightZone.Coast, Temperature = TemperatureZone.Tropical },

            new Biome { Name = "Snow Desert", Color =  new Color4(255, 255, 255, 255),
                        Height = HeightZone.Valley, Temperature = TemperatureZone.Polar, Humidity = HumidityZone.Arid },
            new Biome { Name = "Desert", Color =  new Color4(255, 225, 141, 255),
                        Height = HeightZone.Valley, Temperature = TemperatureZone.Tropical, Humidity = HumidityZone.Arid },

            new Biome { Name = "Cold Plains", Color = new Color4(50, 165, 85, 255),
                        Height = HeightZone.Lowland, Temperature = TemperatureZone.Boreal, Humidity = HumidityZone.Normal },
            new Biome { Name = "Warm Plains", Color = new Color4(46, 173, 27, 255),
                        Height = HeightZone.Lowland, Temperature = TemperatureZone.Warm, Humidity = HumidityZone.Normal },
            new Biome { Name = "Desert", Color =  new Color4(255, 225, 141, 255),
                        Height = HeightZone.Lowland, Temperature = TemperatureZone.Tropical, Humidity = HumidityZone.Arid },

            new Biome { Name = "Tundra", Color = new Color4(17, 80, 70, 255),
                        Height = HeightZone.Valley, Temperature = TemperatureZone.Boreal, Humidity = HumidityZone.Arid },
            new Biome { Name = "Forest", Color = new Color4(28, 111, 44, 255),
                        Height = HeightZone.Valley, Temperature = TemperatureZone.Warm, Humidity = HumidityZone.Normal },
            new Biome { Name = "Rain Forest", Color = new Color4(60, 111, 44, 255),
                        Height = HeightZone.Valley, Temperature = TemperatureZone.Tropical, Humidity = HumidityZone.Humid },

            new Biome { Name = "Lower Mountains", Color = new Color4(130, 109, 88, 255),
                        Height = HeightZone.Upland, Temperature = TemperatureZone.Tropical },
            new Biome { Name = "Upper Mountains", Color = new Color4(171, 142, 112, 255),
                        Height = HeightZone.Mountain, Temperature = TemperatureZone.Tropical },

            new Biome { Name = "Snow Peaks", Color = new Color4(255, 255, 255, 255),
                        Height = HeightZone.Peak, Temperature = TemperatureZone.Cool, Humidity = HumidityZone.Normal },
            new Biome { Name = "Peaks", Color = new Color4(171, 142, 112, 255),
                        Height = HeightZone.Peak, Temperature = TemperatureZone.Tropical, Humidity = HumidityZone.Normal  },
            new Biome { Name = "Rain Peaks", Color = new Color4(28, 111, 44, 255),
                        Height = HeightZone.Peak, Temperature = TemperatureZone.Tropical, Humidity = HumidityZone.Humid },
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
                float height = MathF.Pow(_heightMap[x, z] * 4, 4f);
                height += MathF.Pow(_heightMap[x, z] * 8, 2f);
                height *= 1.5f;
                //float height = _heightMap[x, z] * HeightMultiplier;

                _meshData.Vertices[i] = new Vector3(topLeftX - x, height, topLeftZ - z);
                _meshData.UV[i] = new Vector2(x / (float)_width, z / (float)_height);
                _meshData.AmbientOcclusion[i] = CalculateAO(x, z);

                if (x < _width && z < _height - 1)
                {
                    _meshData.AddTriangle(vertex + 1, vertex, vertex + _width + 1);
                    _meshData.AddTriangle(vertex + _width + 2, vertex + 1, vertex + _width + 1);
                }
            }
        }
    }

    private float CalculateAO(int x, int z)
    {
        float ao = 0f;
        int count = 0;

        for (int dz = -1; dz <= 1; dz++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                if (dx == 0 && dz == 0)
                    continue;

                int nx = x + dx;
                int nz = z + dz;

                if (nx >= 0 && nz >= 0 && nx < _width - 1 && nz < _height - 1)
                {
                    float height = _heightMap[x, z];
                    float neighborHeight = _heightMap[nx, nz];

                    if (neighborHeight > height)
                    {
                        //ao += Math.Clamp(neighborHeight - height, 0, 1);
                        ao += Math.Clamp((neighborHeight - height) * 4f, 0, 1);
                        //ao += neighborHeight * 2f - height * 2f;
                    }
                    count++;
                }
            }
        }

        ao = Math.Clamp(ao / count, 0, 1);
        return 1f - ao;
    }

    protected override void FixedUpdate()
    {
        //if (Input.GetKeyDown(Keys.D1))
        //    _drawMode = DrawMode.ColorMap;
        //if (Input.GetKeyDown(Keys.D2))
        //    _drawMode = DrawMode.HeightMap;
        //if (Input.GetKeyDown(Keys.D3))
        //    _drawMode = DrawMode.TemperatureMap;
        //if (Input.GetKeyDown(Keys.D4))
        //    _drawMode = DrawMode.HumidityMap;

        //Game.Instance.Title += $" | {_drawMode}";

        //if (_previousDrawMode != _drawMode)
        //    _renderer.Material!.Texture = _textures[_drawMode];

        //_previousDrawMode = _drawMode;
    }

    protected override void Dispose(bool disposing)
    {
        _meshData?.Dispose();
        _heightMap = new float[0, 0];
        Biomes = [];
    }
}