using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Blyskavitsya;
public static class Settings
{
    private static Dictionary<string, Keys> _keyBindings = new()
    {
        { "Forward", Keys.W },
        { "Backward", Keys.S },
        { "Left", Keys.A },
        { "Right", Keys.D },
    };
    private static Dictionary<string, MouseButton> _mouseBindings = new()
    {
        { "Attack", MouseButton.Left },
        { "Use", MouseButton.Right },
    };

    internal static GameState GameState { get; private set; } = GameState.Running;

    public static Vector2i Resolution { get; set; } = new Vector2i(1280, 720);
    public static float AspectRatio => Resolution.X / Resolution.Y;

    public static void SetKey(string name, Keys value) => _keyBindings[name] = value;
    public static Keys GetKey(string name) => _keyBindings[name];
    public static Dictionary<string, Keys> GetKeyBindings() => _keyBindings;

    public static void GetButton(string name, MouseButton value) => _mouseBindings[name] = value;
    public static MouseButton GetButton(string name) => _mouseBindings[name];
    public static Dictionary<string, MouseButton> GetMouseBindings() => _mouseBindings;

    public static void PauseGame() => GameState = GameState.Paused;
    public static void UnpauseGame() => GameState = GameState.Running;
}
