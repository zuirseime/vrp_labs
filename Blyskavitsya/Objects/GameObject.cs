using Blyskavitsya.Components;
using Blyskavitsya.Graphics;
using OpenTK.Mathematics;

namespace Blyskavitsya;
public class GameObject : Objects.Object
{
    private bool _visible = true;

    internal List<Component> Components { get; private set; } = [];

    public Transform Transform => GetComponent<Transform>()!;
    public bool Visible => _visible;

    public GameObject(string name, GameObject? parent = null) : base(name)
    {
        AddComponent<Transform>();
        
        if (parent is not null)
        {
            Transform.Parent = parent.Transform;
            parent.Transform.Children.Add(Transform);
        }
        else
            Game.Instance.CurrentScene.EntityPool.Add(this);
    }

    public void SetVisibility(bool visible) => _visible = visible;

    protected override void Start()
    {
        Components.ForEach(x => x.OnStart());
        Transform.Children.ForEach(c => c.GameObject.OnStart());
    }

    protected override void EarlyUpdate()
    {
        if (!Visible) return;

        Components.ForEach(c => c.OnEarlyUpdate());
        Transform.Children.ForEach(c => c.GameObject.OnEarlyUpdate());
    }

    protected override void Update()
    {
        if (!Visible) return;

        Components.ForEach(c => c.OnUpdate());
        Transform.Children.ForEach(c => c.GameObject.OnUpdate());
    }

    protected override void FixedUpdate()
    {
        if (!Visible) return;

        Components.ForEach(c => c.OnFixedUpdate());
        Transform.Children.ForEach(c => c.GameObject.OnFixedUpdate());
    }

    protected override void LateUpdate()
    {
        if (!Visible) return;

        Components.ForEach(c => c.OnLateUpdate());
        Transform.Children.ForEach(c => c.GameObject.OnLateUpdate());
    }

    protected override void Render()
    {
        if (!Visible) return;

        var renderer = GetComponent<Renderer>();
        var camera = Camera.MainCamera;

        if (renderer is not null && camera is not null)
        {
            Matrix4 model = Transform.WorldMatrix;
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjectionMatrix();

            if (renderer.Material is not null)
            {
                renderer.Material.SetUniform(UniformType.Matrix4, nameof(model), model);
                renderer.Material.SetUniform(UniformType.Matrix4, nameof(view), view);
                renderer.Material.SetUniform(UniformType.Matrix4, nameof(projection), projection);
            }
        }

        Components.ForEach(c => c.OnRender());
        Transform.Children.ForEach(c => c.GameObject.OnRender());
    }

    public void LookAt(Vector3 target)
    {
        Vector3 forward = (target - Transform.Position).Normalized();
        Vector3 right = Vector3.Cross(Transform.Up, forward).Normalized();
        Vector3 up = Vector3.Cross(forward, right);

        Matrix3 rotation = new(right, up, forward);
        Transform.Rotation = Quaternion.FromMatrix(rotation);
    }

    public T AddComponent<T>() where T : Component, new()
    {
        T component = new();
        component.SetEntity(this);
        Components.Add(component);

        return component;
    }

    public void AddComponent<T>(T component) where T : Component, new()
    {
        component.SetEntity(this);
        Components.Add(component);
    }

    public void RemoveComponent<T>() where T : Component
    {
        var component = GetComponent<T>();
        if (component is null) return;

        Components.Remove(component);
    }

    public static List<T> FindObjectsOfType<T>() where T : Objects.Object
    {
        List<Objects.Object> objects = [];
        foreach (var child in Game.Instance.CurrentScene.EntityPool)
        {
            if (child is not GameObject goChild)
                continue;

            if (!goChild.Visible)
                continue;

            objects.AddRange(child.Flatten());
        }

        return objects.Where(x => x is T).Cast<T>().ToList();
    }

    public static T? FindObjectOfType<T>() where T : Objects.Object
    {
        return FindObjectsOfType<T>().FirstOrDefault();
    }

    public T? GetComponent<T>() where T : Component 
        => (T?)Components.FirstOrDefault(c => c.GetType() == typeof(T) || c.GetType().IsSubclassOf(typeof(T)));
    public IEnumerable<Component> GetComponents() => Components;
    public IEnumerable<T> GetComponents<T>() where T : Component => Components.OfType<T>();

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

    public List<Objects.Object> Flatten()
    {
        List<Objects.Object> flatten = [this];
        Components.ForEach(flatten.Add);

        foreach (var child in Transform.Children)
        {
            flatten.AddRange(child.GameObject.Flatten());
        }

        return flatten;
    }

    public void Destroy(GameObject gameObject)
    {
        Transform.Children.Remove(gameObject.Transform);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Transform.Children.ForEach(c => c.Dispose());
            Components.ForEach(c => c.Dispose());
        }
    }
}
