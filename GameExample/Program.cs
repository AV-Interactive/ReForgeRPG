using ReForge.Engine.Core;

namespace GameExample;

class Program
{
    static void Main(string[] args)
    {
        var engine = new Engine(800, 600, "Mon premier jeu avec ReForgeRPG");
        engine.Initialize();
        engine.Run();
    }
}