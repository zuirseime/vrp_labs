using OpenTK.Mathematics;

namespace Blyskavitsya;
public class Camera : Component
{
    public float FieldOfView { get; set; } = Globals.FieldOfView;
    public float DepthNear { get; set; } = 0.01f;
    public float DepthFar { get; set; } = 100.0f;
    public Color4 Background { get; set; } = Color4.Blue;

    public static Camera MainCamera { get; private set; } = null!;
    public void SetMain() => MainCamera = this;

    internal Matrix4 GetViewMatrix()
        => Matrix4.LookAt(Transform.Position, Transform.Position + Transform.Forward, Transform.Up);

    internal Matrix4 GetProjectionMatrix() 
        => Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(FieldOfView), AspectRatio, DepthNear, DepthFar);
}
