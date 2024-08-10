namespace Blyskavitsya;
public sealed class GameObject(string name) : IDisposable
{
    private bool _disposed;
    private List<Component> _components = [new Transform()];

    public string Name { get; private set; } = name;

    public Transform transform => GetComponent<Transform>();

    public void Start() => _components.ForEach(x => x.OnStart());
    public void EarlyUpdate() => _components.ForEach(c => c.OnEarlyUpdate());
    public void Update() => _components.ForEach(c => c.OnUpdate());
    public void FixedUpdate() => _components.ForEach(c => c.OnFixedUpdate());
    public void LateUpdate() => _components.ForEach(c => c.OnLateUpdate());
    public void Render() => _components.ForEach(c => c.OnRender());

    public T AddComponent<T>() where T : Component, new()
    {
        T component = new();
        component.SetEntity(this);
        _components.Add(component);

        return component;
    }

    public T GetComponent<T>() where T : Component => (T)_components.First(c => c.GetType() == typeof(T));

    public bool TryGetComponent<T>(out T? component) where T : Component
    {
        component = null;

        try
        {
            component = GetComponent<T>();
            return true;
        } catch
        {
            return false;
        }
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public override string ToString()
    {
        string result = Name;

        

        return result + '\n';
    }
}
