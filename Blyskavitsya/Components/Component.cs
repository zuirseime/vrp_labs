namespace Blyskavitsya;
public abstract class Component : IDisposable
{
    protected bool _disposed;

    public GameObject gameObject { get; private set; } = null!;
    public Transform transform => gameObject.transform;

    internal void SetEntity(GameObject entity) => this.gameObject = entity;

    protected T AddComponent<T>() where T : Component, new() => gameObject.AddComponent<T>();
    protected T GetComponent<T>() where T : Component => gameObject.GetComponent<T>();
    protected bool TryGetComponent<T>(out T? component) where T : Component => gameObject.TryGetComponent(out component);

    internal void OnStart()
    {
        Start();
    }
    protected virtual void Start() { }

    internal void OnEarlyUpdate()
    {
        EarlyUpdate();
    }
    protected virtual void EarlyUpdate() { }

    internal void OnUpdate()
    {
        Update();
    }
    protected virtual void Update() { }

    internal void OnFixedUpdate()
    {
        FixedUpdate();
    }
    protected virtual void FixedUpdate() { }

    internal void OnLateUpdate()
    {
        LateUpdate();
    }
    protected virtual void LateUpdate() { }

    internal void OnRender()
    {
        Render();
    }
    protected virtual void Render() { }

    protected virtual void Dispose(bool disposing) { }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
