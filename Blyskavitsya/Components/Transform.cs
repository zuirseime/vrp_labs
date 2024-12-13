using OpenTK.Mathematics;

namespace Blyskavitsya;
public class Transform : Component
{
    public Transform? Parent { get; internal set; }
    public List<Transform> Children { get; protected set; } = [];

    public Vector3 LocalPosition { get; set; } = Vector3.Zero;
    public Quaternion LocalRotation { get; set; } = Quaternion.Identity;
    public Vector3 LocalScale { get; set; } = Vector3.One;

    public Vector3 Position
    {
        get => GetGlobalPosition();
        set => SetGlobalPosition(value);
    }

    public Quaternion Rotation
    {
        get => GetGlobalRotation();
        set => SetGlobalRotation(value);
    }

    public Vector3 Scale
    {
        get => GetGlobalScale();
        set => SetGlobalScale(value);
    }

    public Vector3 Forward
    {
        get
        {
            float pitch = MathHelper.DegreesToRadians(Transform.Rotation.X);
            float yaw = MathHelper.DegreesToRadians(Transform.Rotation.Y);

            Vector3 forward = new()
            {
                X = MathF.Cos(yaw) * MathF.Cos(pitch),
                Y = MathF.Sin(pitch),
                Z = MathF.Sin(yaw) * MathF.Cos(pitch)
            };

            return Vector3.Normalize(forward);
        }
    }
    public Vector3 Up => Vector3.Normalize(Vector3.Cross(Transform.Right, Transform.Forward));
    public Vector3 Right => Vector3.Normalize(Vector3.Cross(Transform.Forward, WorldUp));

    public static Vector3 WorldForward = -Vector3.UnitZ;
    public static Vector3 WorldRight = Vector3.UnitX;
    public static Vector3 WorldUp = Vector3.UnitY;

    protected override void Start()
    {
        MarkDirty();
    }

    private Vector3 GetGlobalPosition()
    {
        if (GameObject.Transform.Parent is not Transform parent) return LocalPosition;
        return Vector3.TransformPosition(LocalPosition, parent.Transform.WorldMatrix);
    }

    private Quaternion GetGlobalRotation()
    {
        if (GameObject.Transform.Parent is not Transform parent) return LocalRotation;
        return parent.Transform.Rotation * LocalRotation;
    }

    private Vector3 GetGlobalScale()
    {
        if (GameObject.Transform.Parent is not Transform parent) return LocalScale;
        return parent.Transform.Scale * LocalScale;
    }

    private void SetGlobalPosition(Vector3 globalPosition)
    {
        if (GameObject.Transform.Parent is not Transform parent)
        {
            if (LocalPosition == globalPosition)
                return;
            LocalPosition = globalPosition;
        } else
        {
            var inverseParentMatrix = Matrix4.Invert(parent.Transform.WorldMatrix);
            var newLocalPosition = Vector3.TransformPosition(globalPosition, inverseParentMatrix);
            if (LocalPosition == newLocalPosition)
                return;
            LocalPosition = newLocalPosition;
        }
        MarkDirty();
    }

    private void SetGlobalRotation(Quaternion globalRotation)
    {
        if (GameObject.Transform.Parent is not Transform parent) LocalRotation = globalRotation;
        else LocalRotation = Quaternion.Invert(parent.Transform.Rotation) * globalRotation;
        MarkDirty();
    }

    public void SetGlobalScale(Vector3 globalScale)
    {
        if (GameObject.Transform.Parent is not Transform parent) LocalScale = globalScale;
        else LocalScale = globalScale / parent.Transform.Scale;
        MarkDirty();
    }

    private Matrix4? _cachedWorldMatrix = null;
    private bool _isDirty = true;

    internal Matrix4 WorldMatrix
    {
        get
        {
            if (_isDirty)
            {
                Matrix4 translation = Matrix4.CreateTranslation(Transform.LocalPosition);
                Matrix4 rotation = Matrix4.CreateFromQuaternion(Transform.LocalRotation);
                Matrix4 scale = Matrix4.CreateScale(Transform.LocalScale);

                _cachedWorldMatrix = scale * rotation * translation;

                if (GameObject.Transform.Parent is Transform parent)
                    _cachedWorldMatrix *= parent.WorldMatrix;

                _isDirty = false;
            }

            return _cachedWorldMatrix!.Value;
        }
    }

    internal void MarkDirty()
    {
        if (_isDirty)
            return;

        _isDirty = true;

        Children.ForEach(c => c.MarkDirty());
    }
}
