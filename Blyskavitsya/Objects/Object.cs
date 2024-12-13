namespace Blyskavitsya.Objects;
public abstract class Object(string name) : IDisposable
{
    protected bool _disposed;

    internal bool Started = false;

    public string Name { get; set; } = name;

    internal virtual void OnStart()
    {
        if (Started) return;
        Started = true;

        Start();
    }
    internal void OnEarlyUpdate()
    {
        if (!Started) return;

        EarlyUpdate();
    }
    internal void OnUpdate()
    {
        if (!Started) return;

        Update();
    }
    internal void OnLateUpdate()
    {
        if (!Started) return;

        LateUpdate();
    }

    internal void OnFixedUpdate()
    {
        if (!Started) return;

        FixedUpdate();
    }

    internal void OnRender() => Render();

    protected virtual void Start() { }
    protected virtual void EarlyUpdate() {}
    protected virtual void Update() { }
    protected virtual void FixedUpdate() { }
    protected virtual void LateUpdate() { }
    protected virtual void Render() { }

    //protected static Object? FindObjectOfName(string name)
    //{
    //    IEnumerable<Object> objects = [];
    //    foreach (var child in Game.Instance.ScenePool)
    //    {
    //        objects = objects.Concat(child.Flatten());
    //    }

    //    return objects.Where(x => x.Name == name).FirstOrDefault();
    //}

    //public List<Object> Flatten()
    //{
    //    List<Object> flatten = [this];

    //    foreach (var child in Children)
    //    {
    //        flatten.AddRange(child.Flatten());
    //        if (child is GameObject go)
    //        {
    //            flatten.AddRange(go.Components.SelectMany(x => x.Flatten()));
    //        }
    //    }

    //    return flatten;
    //}

    public void Dispose()
    {
        if (!_disposed)
        {
            Dispose(disposing: true);
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing) { }
}
