namespace Blyskavitsya;
public abstract class Scene : IDisposable
{
    protected bool _disposed;
    protected List<GameObject> _entityPool = [];

    private double _fixedTimeStep = 1d / 50d;
    private double _accumulator = 0d;

    public Camera MainCamera { get; private set; }
    public string Name { get; protected set; } = "Scene";

    protected Scene()
    {
        Game.Instance.ScenePool.Add(this);
    }

    public void Navigate(string sceneName)
    {
        Game.Instance.CurrentScene = Game.Instance.ScenePool.First(s => s.Name == sceneName);
        Game.Instance.CurrentScene.OnStart();
        Dispose();
    }

    internal void OnStart()
    {
        GameObject camera = new("MainCamera");
        MainCamera = camera.AddComponent<Camera>();
        MainCamera.FieldOfView = 90;
        camera.AddComponent<CameraMovement>();
        _entityPool.Add(camera);

        Start();

        _entityPool.ForEach(e => e.Start());

        Console.WriteLine(Name);
        _entityPool.ForEach(Console.WriteLine);
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
        Game.Instance.Title = $"{Name} - {MainCamera.transform.Position}";
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
