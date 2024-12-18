using OpenTK.Mathematics;

namespace Labs.Components;
public class Biome
{
    public string? Name { get; set; }
    public Color4 Color { get; set; }
    public HeightZone Height { get; set; } = HeightZone.Peak;
    public TemperatureZone Temperature { get; set; } = TemperatureZone.Tropical;
    public HumidityZone Humidity { get; set; } = HumidityZone.Humid;
}

public enum HeightZone : short
{
    Ocean = 20,
    Sea = 39,
    Coast = 41,
    Lowland = 45,
    Valley = 55,
    Upland = 65,
    Mountain = 85,
    Peak = 100
}

public enum TemperatureZone : short
{
    Polar = 10,
    Boreal = 30,
    Cool = 60,
    Warm = 90,
    Tropical = 100
}

public enum HumidityZone : short
{
    Arid = 15,
    Normal = 85,
    Humid = 100,
}