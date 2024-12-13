using Blyskavitsya.Graphics;
using Blyskavitsya.Utilities;
using OpenTK.Mathematics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blyskavitsya;
public static class Resources
{
    private const string RESOURCES_FOLDER = "Resources";

    private static readonly List<Resource> resources = [];
    private static readonly JsonSerializerOptions jsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter(), new Color4Converter() }
    };

    public static T? GetResource<T>(string name) where T : Resource
        => (T?)resources.Find(r => r.Name == name);

    public static List<T> GetResourcesOfType<T>() where T : Resource
           => resources.OfType<T>().ToList();

    public static void LoadResources()
    {
        LoadShaders();
        LoadTextures();
        LoadMaterials();

        Console.WriteLine();
    }

    private static void LoadShaders()
    {
        string folder = GetFolder(ResourceType.Shaders);
        var vertexShaders = Directory.GetFiles(folder, "*.vert");
        var fragmentShaders = Directory.GetFiles(folder, "*.frag");

        foreach (var vertexShader in vertexShaders)
        {
            string name = GetName(vertexShader);
            string? fragmentShader = fragmentShaders.FirstOrDefault(f => f.Contains(name));

            if (fragmentShader is null)
            {
                Console.WriteLine($"Warning: Fragment shader for '{name}' not found.");
                continue;
            }

            Shader shader = new(vertexShader, fragmentShader) { Name = name };
            resources.Add(shader);
            Console.WriteLine($"Loaded Shader: {shader.Name}");
        }
    }

    private static void LoadTextures()
    {
        string folder = GetFolder(ResourceType.Textures);
        var textures = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);

        foreach (var texturesFile in textures)
        {
            string name = GetName(texturesFile);
            Texture texture = new(texturesFile) { Name = name };
            resources.Add(texture);
            Console.WriteLine($"Loaded Texture: {texture.Name}");
        }
    }

    private static void LoadMaterials()
    {
        string floader = GetFolder(ResourceType.Materials);
        var materials = Directory.GetFiles(floader, "*.json", SearchOption.AllDirectories);

        foreach (var materialFile in materials)
        {
            string json = File.ReadAllText(materialFile);
            Material material = LoadMaterial(json);
            material.Name = GetName(materialFile);
            resources.Add(material);
            Console.WriteLine($"Loaded Material: {material.Name}");
        }
    }

    public static Material LoadMaterial(string json)
    {
        var data = JsonSerializer.Deserialize<MaterialData>(json, jsonOptions) 
            ?? throw new Exception("Failed to load material from JSON.");

        Shader shader = GetResource<Shader>(data.Shader)
            ?? throw new Exception($"Shader '{data.Shader}' not found.");

        Texture? texture = data.Texture != null ? GetResource<Texture>(data.Texture) : null;

        return new Material(shader, data.Type)
        {
            Color = new Color4(data.Color.R, data.Color.G, data.Color.B, data.Color.A),
            Texture = texture,
            Name = data.Name,
        };
    }

    private static string GetFolder(ResourceType type)
    {
        string folder = Path.Combine(RESOURCES_FOLDER, type.ToString());
        Directory.CreateDirectory(folder);
        return folder;
    }

    private static string GetName(string file) 
        => Path.GetFileNameWithoutExtension(file);
}

internal enum ResourceType
{
    None, Shaders, Textures, Icons, Materials
}

internal class MaterialData
{
    public string Name { get; set; } = null!;
    public string Shader { get; set; } = null!;
    public MaterialType Type { get; set; }
    public string? Texture { get; set; }
    public Color4 Color { get; set; }
}