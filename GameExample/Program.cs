using System.Numerics;
using GameExample.Factories;
using ReForge.Engine.Core;
using ReForge.Engine.World;
using ReForge.Engine.World.Behaviors;

namespace GameExample;

class Program
{
    static void Main(string[] args)
    {
        var engine = new Engine(800, 600, "L'Éveil du Gardien - ReForgeRPG");
        engine.Initialize();
        
        EveilDuGardien.Setup(engine);
        
        engine.Run();
    }
}