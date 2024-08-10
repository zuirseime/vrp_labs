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

    public static void Initialize(GameWindow window)
    {
        window.KeyDown += OnKeyDown;
        window.KeyUp += OnKeyUp;
        window.MouseDown += OnMouseDown;
        window.MouseUp += OnMouseUp;
        window.MouseMove += OnMouseMove;
    }

    private static void OnKeyDown(KeyboardKeyEventArgs args) => _keys.Add(args.Key);
    private static void OnKeyUp(KeyboardKeyEventArgs args) => _keys.Remove(args.Key);
    public static bool GetKeyDown(Keys key) => _keys.Contains(key);

    private static void OnMouseDown(MouseButtonEventArgs e) => _mouseButtons.Add(e.Button);
    private static void OnMouseUp(MouseButtonEventArgs e) => _mouseButtons.Remove(e.Button);
    private static void OnMouseMove(MouseMoveEventArgs e)
    {
        _mouseDelta = e.Position - _lastMousePosition;
        _lastMousePosition = e.Position;
    }
    public static bool GetButtonDown(MouseButton button) => _mouseButtons.Contains(button);

    public static float GetAxis(Axes axis)
    {
        float value = 0f;

        switch (axis)
        {
            case Axes.Horizontal:
                if (GetKeyDown(Settings.GetKey("Left")))
                    value -= 1f;
                if (GetKeyDown(Settings.GetKey("Right")))
                    value += 1f;
                break;
            case Axes.Vertical:
                if (GetKeyDown(Settings.GetKey("Forward")))
                    value += 1f;
                if (GetKeyDown(Settings.GetKey("Backward")))
                    value -= 1f;
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
