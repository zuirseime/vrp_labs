namespace Blyskavitsya.Utilities;
public static class MathFHelper
{
    public static float InverseLerp(float a, float b, float value)
    {
        if (a != b)
            return ClampFloat((value - a) / (b - a));
        return 0.0f;
    }

    public static float ClampFloat(float value)
    {
        if (value < 0.0f)
            return 0.0f;
        if (value > 1.0f)
            return 1.0f;
        return value;
    }
}
