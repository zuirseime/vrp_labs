using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using Blyskavitsya.Components;
using Blyskavitsya.Graphics;
using System.Diagnostics;

namespace Blyskavitsya;

public class Game : GameWindow
{
    private double _fixedTimeStep = 20d;
    private double _accumulator = 0d;

    internal List<Scene> ScenePool { get; set; } = [];
    public Scene CurrentScene { get; set; }

    public static Game Instance { get; private set; } = null!;

    public Game(int width, int height)
        : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = new Vector2i(width, height)})
    {
        Resolution = new Vector2i(width, height);
        Instance = this;
    }

    protected override void OnLoad()
    {
        Stopwatch sw = Stopwatch.StartNew();

        base.OnLoad();

        Resources.LoadResources();

        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        CenterWindow();

        Input.Initialize(this);
        CurrentScene.OnStart();

        GL.Enable(EnableCap.DepthTest);

        sw.Stop();

        Console.WriteLine($"Launched in {sw.Elapsed} seconds");
    }

    protected sealed override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, e.Width, e.Height);
        Resolution = new Vector2i(e.Width, e.Height);
    }

    protected sealed override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        if (CurrentGameState == GameState.Running)
        {
            GL.ClearColor(Camera.MainCamera.Background);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            CurrentScene.OnRender();

            SwapBuffers();

            Time.CalcualateFramesPerSecond(args.Time);
        }

    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        CurrentScene.OnEarlyUpdate();

        CurrentScene.OnUpdate();

        _accumulator += args.Time;
        if (_accumulator >= _fixedTimeStep)
        {
            CurrentScene.OnFixedUpdate();
            
            _accumulator = 0;
        }

        CurrentScene.OnLateUpdate();

        Input.Update();

        if (CurrentGameState != GameState.Running)
            Time.DropDelta();
    }

    protected override void OnUnload()
    {
        base.OnUnload();

        ScenePool.ForEach(s => s.Dispose());
    }
}
