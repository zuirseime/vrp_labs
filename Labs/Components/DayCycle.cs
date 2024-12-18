using Blyskavitsya.Components;
using Blyskavitsya.Graphics;
using Blyskavitsya.Objects;
using Blyskavitsya;
using OpenTK.Mathematics;

namespace Labs.Components;
public struct SkyGradient(Color4 top, Color4 bottom)
{
    public Color4 Top { get; set; } = top;
    public Color4 Bottom { get; set; } = bottom;
}

public class DayCycle : Component
{
    private DateTime _startTime;
    private readonly static float DayMinutes = 5f;
    private readonly float DayDuration = (float)TimeSpan.FromMinutes(DayMinutes).TotalSeconds;
    private float _currentTime;
    private List<float> _timeThresholds = [];

    private Sun _sun;
    private Moon _moon;

    private readonly Dictionary<float, SkyGradient> _gradients = new()
    {
        {
            0.1f,
            new SkyGradient(new Color4(0.3f, 0.6f, 0.9f, 1.0f), new Color4(0.9f, 0.7f, 0.5f, 1.0f))
        },
        {
            0.4f,
            new SkyGradient(new Color4(0.2f, 0.5f, 1.0f, 1.0f), new Color4(0.8f, 0.8f, 0.8f, 1.0f))
        },
        {
            0.5f,
            new SkyGradient(new Color4(0.2f, 0.1f, 0.3f, 1.0f), new Color4(0.5f, 0.3f, 0.3f, 1.0f))
        },
        {
            0.6f,
            new SkyGradient(new Color4(0.05f, 0.1f, 0.3f, 1.0f), new Color4(0.1f, 0.1f, 0.2f, 1.0f))
        },
        {
            1f,
            new SkyGradient(new Color4(0.05f, 0.1f, 0.3f, 1.0f), new Color4(0.1f, 0.1f, 0.2f, 1.0f))
        },
    };

    private Renderer? _renderer;

    protected override void Start()
    {
        var skyBox = GameObject.FindObjectOfType<SkyBox>();
        _renderer = skyBox?.GetComponent<Renderer>();

        foreach (var key in _gradients.Keys)
            _timeThresholds.Add(key);
        _timeThresholds.Sort();

        _sun = new(parent: GameObject);
        _sun.Transform.LocalPosition = Transform.WorldForward * 2f;
        var sunRenderer = _sun.AddComponent<Renderer>();
        sunRenderer.Material = Resources.GetResource<Material>("Sun");

        _moon = new(parent: GameObject);
        _moon.Transform.LocalPosition = Transform.WorldForward * -2.5f;
        var moonRenderer = _moon.AddComponent<Renderer>();
        moonRenderer.Material = Resources.GetResource<Material>("Moon");

        _startTime = DateTime.Now.AddMinutes(-2);
    }

    protected override void Update()
    {
        Game.Instance.Title = $" | Day progress: {Math.Round(_currentTime, 2)}";

        UpdateRotation();
        UpdateSkyBox();

        _currentTime = GetDayProgress();
    }

    private float GetDayProgress()
    {
        TimeSpan elapsed = DateTime.Now - _startTime;
        return (float)(elapsed.TotalSeconds % DayDuration) / DayDuration;
    }

    private void UpdateRotation()
    {
        float angle = _currentTime * 360f;
        Transform.Rotation = Quaternion.FromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(angle));
    }

    private void UpdateSkyBox()
    {
        var gradient = GetCurrentGradient();

        if (_renderer is not null && _renderer.Material is not null)
        {
            _renderer.Material.SetUniform(UniformType.Color4, nameof(SkyGradient.Top), gradient.Top);
            _renderer.Material.SetUniform(UniformType.Color4, nameof(SkyGradient.Bottom), gradient.Bottom);
        }

        var renderers = GameObject.FindObjectsOfType<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            renderer.Material?.SetUniform(UniformType.Color4, "fogColor", gradient.Bottom);
        }
    }

    private SkyGradient GetCurrentGradient()
    {
        for (int i = 0; i < _timeThresholds.Count; i++)
        {
            float startThreshold = _timeThresholds[i];
            int next = (i + 1) % _timeThresholds.Count;
            float endThreshold = _timeThresholds[next];

            float tempEnd = endThreshold + (next == 0 ? 1 : 0);
            float min = MathF.Min(startThreshold, tempEnd);
            float max = MathF.Max(startThreshold, tempEnd);

            float now = _currentTime + (next == 0 ? 1 : 0);

            if (now >= min && now < max)
            {
                float t = (now - min) / (max - min);

                SkyGradient startGradient = _gradients[startThreshold];
                SkyGradient endGradient = _gradients[endThreshold];

                return new SkyGradient(
                    Extensions.Lerp(startGradient.Top, endGradient.Top, t),
                    Extensions.Lerp(startGradient.Bottom, endGradient.Bottom, t)
                );
            }
        }

        return default;
    }
}
