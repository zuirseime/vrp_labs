using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;

namespace Blyskavitsya;

public class Game : GameWindow
{
    private double _fixedTimeStep = 1d / 50d;
    private double _accumulator = 0d;

    internal List<Scene> ScenePool { get; set; } = [];
    public Scene CurrentScene { get; set; }

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

    protected sealed override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, e.Width, e.Height);
        Settings.Resolution = new Vector2i(e.Width, e.Height);
    }

    protected sealed override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        CurrentScene.OnRender();

        Time.CalcualateFramesPerSecond(args.Time);
        SwapBuffers();
    }

    protected sealed override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        CurrentScene.OnEarlyUpdate();

        _accumulator += args.Time;
        while (_accumulator >= _fixedTimeStep)
        {
            CurrentScene.OnFixedUpdate();
            _accumulator -= _fixedTimeStep;
        }

        CurrentScene.OnUpdate();

        CurrentScene.OnLateUpdate();

        Input.Update();
    }

    protected override void OnUnload()
    {
        base.OnUnload();

        CurrentScene.Dispose();
    }
}
