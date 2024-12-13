using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Blyskavitsya;
public static class Input
{
    private static HashSet<Keys> _keys = [];
    private static HashSet<MouseButton> _mouseButtons = [];
    private static Vector2 _mouseDelta;
    private static Vector2 _lastMousePosition = Vector2.Zero;
    private static bool _firstMove = true;
    private static Vector2 _wheelOffset = Vector2.Zero;

    internal static void Initialize(GameWindow window)
    {
        window.KeyDown += OnKeyDown;
        window.KeyUp += OnKeyUp;
        window.MouseDown += OnMouseDown;
        window.MouseUp += OnMouseUp;
        window.MouseMove += OnMouseMove;
        window.MouseWheel += OnMouseWheel;
    }

    private static void OnKeyDown(KeyboardKeyEventArgs args) => _keys.Add(args.Key);
    private static void OnKeyUp(KeyboardKeyEventArgs args) => _keys.Remove(args.Key);
    public static bool GetKeyDown(Keys key) => _keys.Contains(key);
    public static bool GetKeyPressed(Keys key)
    {
        if (GetKeyDown(key))
        {
            _keys.Remove(key);
            return true;
        }
        return false;
    }

    private static void OnMouseDown(MouseButtonEventArgs e) => _mouseButtons.Add(e.Button);
    private static void OnMouseUp(MouseButtonEventArgs e) => _mouseButtons.Remove(e.Button);
    private static void OnMouseMove(MouseMoveEventArgs e)
    {
        if (_firstMove)
        {
            _lastMousePosition = new Vector2(e.X, e.Y);
            _firstMove = false;
        }
        else
        {
            _mouseDelta = e.Position - _lastMousePosition;
            _lastMousePosition = e.Position;
        }
    }
    private static void OnMouseWheel(MouseWheelEventArgs e) => _wheelOffset = e.Offset;

    public static bool GetButtonDown(MouseButton button) => _mouseButtons.Contains(button);
    public static Vector2 GetWheelOffset() => _wheelOffset;

    public static float GetAxis(Axes axis)
    {
        float value = 0f;

        switch (axis)
        {
            case Axes.Horizontal:
                if (GetKeyDown(Globals.GetKey("Left")))
                    value -= Transform.WorldRight.X;
                if (GetKeyDown(Globals.GetKey("Right")))
                    value += Transform.WorldRight.X;
                break;
            case Axes.Vertical:
                if (GetKeyDown(Globals.GetKey("Forward")))
                    value -= Transform.WorldForward.Z;
                if (GetKeyDown(Globals.GetKey("Backward")))
                    value += Transform.WorldForward.Z;
                break;
            case Axes.MouseX:
                value = GetMouseDelta().X;
                break;
            case Axes.MouseY:
                value = GetMouseDelta().Y;
                break;
            default:
                break;
        }

        return value;
    }

    public static Vector2 GetMouseDelta()
    {
        var delta = _mouseDelta;
        return delta;
    }

    internal static void Update()
    {
        _mouseDelta = Vector2.Zero;
    }
}
