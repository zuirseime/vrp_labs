using Blyskavitsya;
using Labs.Scenes;
using OpenTK.Windowing.Common;

namespace Labs;
public class Sim : Game
{
    public Sim(int width, int height) : base(width, height) { }

    protected override void OnLoad()
    {
        // Change scene type to control it
        CurrentScene = new SimScene();

        // Write your code here

        base.OnLoad();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        if (Globals.CurrentGameState == GameState.Running)
            CursorState = CursorState.Grabbed;
        else
            CursorState = CursorState.Normal;
    }

    protected override void OnUnload()
    {
        base.OnUnload();

        // Write your code here
    }
}
