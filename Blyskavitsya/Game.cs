using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;

namespace Blyskavitsya;

public class Game : GameWindow
{
    private double _fixedTimeStep = 1d / 50d;
    private double _accumulator = 0d;

    internal static Game Instance { get; private set; } = null!;

    public Game(int width, int height)
        : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = new Vector2i(width, height)})
    {
        Instance = this;
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        CenterWindow();

        CursorState = CursorState.Grabbed;

        Input.Initialize(this);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, e.Width, e.Height);
        Settings.Resolution = new Vector2i(e.Width, e.Height);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        Time.CalcualateFramesPerSecond(args.Time);

        base.OnRenderFrame(args);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        Input.Update();

        base.OnUpdateFrame(args);
    }

    protected override void OnUnload()
    {
        base.OnUnload();
    }
}
