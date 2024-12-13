namespace Blyskavitsya;
public class Scene : Objects.Object
{
    private double _fixedTimeStep = 1d / 50d;
    private double _accumulator = 0d;

    internal List<GameObject> EntityPool = [];

    public Scene(string name) : base(name)
    {
        Game.Instance.ScenePool.Add(this);
    }

    protected override void Start()
    {
        EntityPool.ForEach(e => e.OnStart());
    }

    protected override void EarlyUpdate()
    {
        EntityPool.ForEach(e => e.OnEarlyUpdate());
    }

    protected override void Update()
    {
        EntityPool.ForEach(e => e.OnUpdate());
    }

    protected override void FixedUpdate()
    {
        EntityPool.ForEach(e => e.OnFixedUpdate());
    }

    protected override void LateUpdate()
    {
        EntityPool.ForEach(e => e.OnLateUpdate());
    }

    protected override void Render()
    {
        EntityPool.ForEach(e => e.OnRender());
    }


    public void Navigate<T>()
    {
        Game.Instance.CurrentScene = Game.Instance.ScenePool.First(s => s.GetType() is T);
        Game.Instance.CurrentScene.OnStart();
        Dispose();
    }
}
