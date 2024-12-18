using Blyskavitsya;
using Blyskavitsya.Components;
using Blyskavitsya.Graphics;
using Blyskavitsya.Objects;
using Labs.Components;
using OpenTK.Mathematics;
namespace Labs.Scenes;
internal class SimScene() : Scene("Main Scene")
{
    protected override void Start()
    {
        GameObject camera = new("MainCamera");
        var camComp = camera.AddComponent<Camera>();
        camComp.SetMain();

        Camera.MainCamera.DepthFar = float.MaxValue / 1000f;
        Camera.MainCamera.Transform.Position = Transform.WorldUp * 100;
        Camera.MainCamera.Transform.Rotation = Quaternion.FromAxisAngle(Vector3.UnitX, 200);
        Camera.MainCamera.Background = new Color4(63, 128, 255, 255);

        SkyBox skyBox = new();
        var skyRenderer = skyBox.AddComponent<Renderer>();
        skyRenderer.Material = Resources.GetResource<Material>("Sky");
        skyBox.Transform.LocalScale = Vector3.One * 10000;

        GameObject sunOrientation = new("Sun Orientation");
        sunOrientation.Transform.Scale = Vector3.One * 1000f;
        sunOrientation.AddComponent<DayCycle>();

        GameObject clouds = new("Clouds");
        var cloudGenerator = clouds.AddComponent<CloudGenerator>();
        cloudGenerator.Material = Resources.GetResource<Material>("Cloud")!;
        clouds.Transform.Scale = Vector3.One * 20;

        GameObject world = new("World");

        GameObject terrain = new("Terrain", world);
        var terrainRenderer = terrain.AddComponent<Renderer>();
        terrainRenderer.Material = Resources.GetResource<Material>("Terrain");
        terrain.AddComponent<TerrainMesh>();

        GameObject water = new("Water", world);
        var waterRenderer = water.AddComponent<Renderer>();
        waterRenderer.Material = Resources.GetResource<Material>("Water");
        water.AddComponent<WaterMesh>();

        GameObject terrainGenerator = new("TerrainGenerator");
        terrainGenerator.AddComponent<MapGenerator>();

        base.Start();
    }
}
