using OpenTK.Mathematics;

namespace Blyskavitsya;
public class Camera : Component
{
    public float FieldOfView { get; set; }
    public float DepthNear { get; set; } = 0.01f;
    public float DepthFar { get; set; } = 100.0f;

    protected override void LateUpdate() => UpdateVectors();

    internal Matrix4 GetViewMatrix()
        => Matrix4.LookAt(transform.Position, transform.Position + transform.Forward, transform.Up);

    internal Matrix4 GetProjectionMatrix()
        => Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(FieldOfView), Settings.AspectRatio, DepthNear, DepthFar);

    private void UpdateVectors()
    {
        Vector3 front = new()
        {
            X = MathF.Cos(MathHelper.DegreesToRadians(transform.Rotation.Y)) * MathF.Cos(MathHelper.DegreesToRadians(transform.Rotation.X)),
            Y = MathF.Sin(MathHelper.DegreesToRadians(transform.Rotation.X)),
            Z = MathF.Sin(MathHelper.DegreesToRadians(transform.Rotation.Y)) * MathF.Cos(MathHelper.DegreesToRadians(transform.Rotation.X))
        };
        transform.Forward = Vector3.Normalize(front);
        transform.Right = Vector3.Normalize(Vector3.Cross(transform.Forward, Vector3.UnitY));
        transform.Up = Vector3.Normalize(Vector3.Cross(transform.Right, transform.Forward));
    }
}
