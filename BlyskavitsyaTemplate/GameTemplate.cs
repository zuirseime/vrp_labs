using Blyskavitsya;

namespace BlyskavitsyaTemplate;
public class GameTemplate : Game
{
    public GameTemplate(int width, int height) : base(width, height) { }

    protected override void OnLoad()
    {
        base.OnLoad();

        // Write your code here

        // Change scene type to control it
        CurrentScene = new Scene();
        CurrentScene.OnStart();
    }

    protected override void OnUnload()
    {
        base.OnUnload();

        // Write your code here
    }
}
