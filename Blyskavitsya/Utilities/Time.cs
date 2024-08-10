namespace Blyskavitsya;
public static class Time
{
    private static int _frameCount;
    private static double _timeElapsed;
    private static double _fps;

    public static float Delta { get; private set; }
    public static float FPS { get; private set; }

    internal static void CalcualateFramesPerSecond(double delta)
    {
        _frameCount++;
        _timeElapsed += delta;
        Delta = (float)delta;

        if (_timeElapsed >= 1d)
        {
            FPS = (float)(_frameCount / _timeElapsed);
            _frameCount = 0;
            _timeElapsed = 0;
        }
    }
}
