namespace Blyskavitsya;
public class Scene : IDisposable
{
    protected bool _disposed;
    protected List<GameObject> _entityPool = [];

    private double _fixedTimeStep = 1d / 50d;
    private double _accumulator = 0d;

    public Camera MainCamera { get; private set; }

    public Scene()
    {
        Game.Instance.ScenePool.Add(this);
    }

    public void Navigate(Type scene)
    {
        Game.Instance.CurrentScene = Game.Instance.ScenePool.First(s => s.GetType() == scene);
        Game.Instance.CurrentScene.OnStart();
        Dispose();
    }

    public void OnStart()
    {
        GameObject camera = new("MainCamera");
        MainCamera = camera.AddComponent<Camera>();
        MainCamera.FieldOfView = 90;
        _entityPool.Add(camera);

        Start();

        _entityPool.ForEach(e => e.Start());
    }
    protected virtual void Start() { }

    internal void OnRender()
    {
        Render();
        _entityPool.ForEach(e => e.Render());
    }
    protected virtual void Render() { }

    internal void OnEarlyUpdate()
    {
        EarlyUpdate();
        _entityPool.ForEach(e => e.EarlyUpdate());
    }
    protected virtual void EarlyUpdate() { }

    internal void OnUpdate()
    {
        _entityPool.ForEach(e => e.Update());
        Update();
    }
    protected virtual void Update() { }

    internal void OnFixedUpdate()
    {
        _entityPool.ForEach(e => e.FixedUpdate());
        FixedUpdate();
    }
    protected virtual void FixedUpdate() { }

    internal void OnLateUpdate()
    {
        _entityPool.ForEach(e => e.LateUpdate());
        LateUpdate();
    }
    protected virtual void LateUpdate() { }

    public void Dispose()
    {
        Dispose(disposing: true);
        _entityPool.ForEach(e => e.Dispose());
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing) { }
}
