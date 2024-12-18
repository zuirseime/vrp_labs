namespace Labs;

internal class Program
{
    static void Main(string[] args)
    {
        using Sim sim = new(1280, 720);
        sim.Run();
    }
}
