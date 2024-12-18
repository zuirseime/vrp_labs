using Blyskavitsya.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Labs.Components;
public static class TexturePainter
{
    public static string MapName { get; set; } = null!;

    public static Texture DrawColorMap(float[,] heightMap, float[,] temperatureMap, float[,] humidityMap, Biome[] biomes)
    {
        int xSize = heightMap.GetLength(0) - 1;
        int zSize = heightMap.GetLength(1) - 1;

        Color4[] colorMap = new Color4[(xSize + 1) * (zSize + 1)];

        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Math.Clamp(heightMap[x, z], -1f, 1f);
                float temperature = Math.Clamp(temperatureMap[x, z], -1f, 1f);
                float humidity = Math.Clamp(humidityMap[x, z], -1f, 1f);

                foreach (var biome in biomes)
                {
                    if (height <= (short)biome.Height / 100f &&
                        (temperature <= (short)biome.Temperature / 100f &
                        humidity <= (short)biome.Humidity / 100f))
                    {
                        colorMap[z * (xSize + 1) + x] = biome.Color;
                        break;
                    }
                }
            }
        }

        return DrawTexture(colorMap, xSize, zSize);
    }

    public static Texture DrawHeightMap(float[,] heightMap)
    {
        int xSize = heightMap.GetLength(0);
        int zSize = heightMap.GetLength(1);

        Color4[] colorMap = new Color4[(xSize + 1) * (zSize + 1)];

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                colorMap[z * (xSize + 1) + x] = Extensions.Lerp(Color4.Black, Color4.White, heightMap[x, z]);
            }
        }

        return DrawTexture(colorMap, xSize, zSize);
    }

    private static Texture DrawTexture(Color4[] colorMap, int width, int height)
    {
        Texture texture = new(width + 1, height + 1)
        {
            FilterMode = TextureFilter.Nearest,
            WrapMode = TextureWrapMode.ClampToEdge
        };

        texture.SetData(colorMap);
        texture.Apply();
        return texture;
    }
}