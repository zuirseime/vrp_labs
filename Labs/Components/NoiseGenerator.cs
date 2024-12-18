using Blyskavitsya.Utilities;
using OpenTK.Mathematics;

namespace Labs.Components;
public static class NoiseGenerator
{
    private static readonly Random random = new();

    private static int width;
    private static int height;

    public static float[,] GenerateMap(int width, int height, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        NoiseGenerator.width = width;
        NoiseGenerator.height = height;

        var octaveOffsets = ShiftOctaves(octaves, offset);

        return GenerateNoise(scale, persistance, lacunarity, octaveOffsets);
    }

    private static Vector2[] ShiftOctaves(int octaves, Vector2 offset)
    {
        Vector2[] offsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = random.Next(short.MinValue, short.MaxValue) + offset.X;
            float offsetY = random.Next(short.MinValue, short.MaxValue) + offset.Y;

            offsets[i] = new Vector2(offsetX, offsetY);
        }

        return offsets;
    }

    private static float[,] GenerateNoise(float scale, float persistance, float lacunarity, Vector2[] octaveOffsets)
    {
        float[,] map = new float[width, height];

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        for (int i = 0; i < octaveOffsets.Length; i++)
        {
            AddOctave(map, halfWidth, halfHeight, scale, persistance, lacunarity, octaveOffsets[i], i + 1);
        }

        return map;
    }

    private static void AddOctave(
        float[,] map, float halfWidth, float halfHeight, float scale,
        float persistance, float lacunarity, Vector2 offset, int octave)
    {
        float amplitude = MathF.Pow(persistance, octave);
        float frequency = MathF.Pow(lacunarity, octave);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float nx = (x - halfWidth) / scale * frequency + offset.X;
                float ny = (y - halfHeight) / scale * frequency + offset.Y;

                float value = Perlin.Noise(nx, ny) + 0.75f;

                map[x, y] += value * amplitude;
            }
        }
    }
}