namespace Blyskavitsya;
public abstract class Component() : Objects.Object(""), IDisposable
{
    public GameObject GameObject { get; private set; } = null!;
    public Transform Transform => GameObject.Transform;

    internal void SetEntity(GameObject entity)
    {
        GameObject = entity;
    }

    public T AddComponent<T>() where T : Component, new() => GameObject.AddComponent<T>();
    public T? GetComponent<T>() where T : Component => GameObject.GetComponent<T>();
    public bool TryGetComponent<T>(out T? component) where T : Component => GameObject.TryGetComponent(out component);

    public void Destroy(GameObject gameObject) => GameObject.Destroy(gameObject);
}
