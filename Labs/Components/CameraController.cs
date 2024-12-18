using Blyskavitsya;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Labs.Components;
public enum Movement
{
    Idle = 0,
    Crawl = 5,
    Walk = 15,
    Sprint = 75,
}

public class CameraController : Component
{
    private float _zoomedFOV = 30f;
    private float _defaultFOV = Globals.FieldOfView;
    private float _zoomSpeed = 10f;

    public float Speed { get; set; }
    public float Sensitivity { get; set; } = 45f;

    protected override void Start()
    {

    }

    protected override void Update()
    {
        OnZoom();
        OnRotate();
        OnMove();

        if (Input.GetKeyPressed(Keys.Escape))
        {
            if (Globals.CurrentGameState == GameState.Running)
                Globals.PauseGame();
            else
                Globals.UnpauseGame();
        }
    }

    private void OnZoom()
    {
        if (Input.GetKeyDown(Keys.Z))
        {
            Camera.MainCamera.FieldOfView = MathHelper.Lerp(
                Camera.MainCamera.FieldOfView, _zoomedFOV, _zoomSpeed * Time.Delta
            );
        } else
        {
            Camera.MainCamera.FieldOfView = MathHelper.Lerp(
                Camera.MainCamera.FieldOfView, _defaultFOV, _zoomSpeed * Time.Delta * 2
            );
        }
    }

    private void OnRotate()
    {
        var sensitivity = Sensitivity * Time.Delta;

        Quaternion rotation = Transform.LocalRotation;

        rotation.Y += Input.GetAxis(Axes.MouseX) * sensitivity;
        rotation.X -= Input.GetAxis(Axes.MouseY) * sensitivity;
        rotation.X = Math.Clamp(rotation.X, -89.9f, 89.9f);

        Transform.LocalRotation = rotation;
    }

    private void OnMove()
    {
        if (Input.GetKeyDown(Keys.LeftShift))
            Speed = (int)Movement.Sprint;
        else if (Input.GetKeyDown(Keys.LeftControl))
            Speed = (int)Movement.Crawl;
        else
            Speed = (int)Movement.Walk;

        var forward = Transform.Forward;
        forward.Y = 0;
        var input = forward.Normalized() * Input.GetAxis(Axes.Vertical) + Transform.Right * Input.GetAxis(Axes.Horizontal);

        Vector3 verticalMovement = Vector3.Zero;
        if (Input.GetKeyDown(Keys.Space))
            verticalMovement += Transform.WorldUp * (int)Movement.Sprint * Time.Delta;
        if (Input.GetKeyDown(Keys.LeftControl))
            verticalMovement -= Transform.WorldUp * (int)Movement.Sprint * Time.Delta;

        float velocity = Speed * Time.Delta;
        Transform.Position += input * velocity + verticalMovement;
    }
}
