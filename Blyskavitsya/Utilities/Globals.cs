using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Blyskavitsya;
public static class Globals
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

    public static GameState CurrentGameState { get; private set; } = GameState.Running;

    public static Vector2i Resolution { get; set; }
    public static float AspectRatio => Resolution.X / (float)Resolution.Y;
    public static float FieldOfView => 90f;

    public static float RenderDistance => 400;

    public static void SetKey(string name, Keys value) => _keyBindings[name] = value;
    public static Keys GetKey(string name) => _keyBindings[name];
    public static Dictionary<string, Keys> GetKeyBindings() => _keyBindings;

    public static void GetButton(string name, MouseButton value) => _mouseBindings[name] = value;
    public static MouseButton GetButton(string name) => _mouseBindings[name];
    public static Dictionary<string, MouseButton> GetMouseBindings() => _mouseBindings;

    public static void PauseGame() => CurrentGameState = GameState.Paused;
    public static void UnpauseGame() => CurrentGameState = GameState.Running;

    public static void CheckGLError(string message)
    {
        OpenTK.Graphics.OpenGL.ErrorCode error = GL.GetError();
        if (error != OpenTK.Graphics.OpenGL.ErrorCode.NoError)
            Console.WriteLine($"OpenGL Error after {message}: {error}");
    }
}
