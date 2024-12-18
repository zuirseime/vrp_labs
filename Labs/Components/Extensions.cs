using Blyskavitsya.Utilities;
using OpenTK.Mathematics;

namespace Labs.Components;
internal static class Extensions
{
    public static Color4 Lerp(Color4 a, Color4 b, float value)
    {
        value = MathFHelper.ClampFloat(value);

        return new Color4(a.R + (b.R - a.R) * value,
                          a.G + (b.G - a.G) * value,
                          a.B + (b.B - a.B) * value,
                          a.A + (b.A - a.A) * value);
    }
}