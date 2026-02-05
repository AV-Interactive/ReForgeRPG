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
        var engine = new Engine(800, 600, "Mon premier jeu avec ReForgeRPG");
        engine.Initialize();
        
        // Création du Player
        var player = EntityFactory.CreateActor(engine, new Vector2(400,300), "Assets/Sprites/player.png", "Toine", new List<string> {"Player"});
        player.AddBehavior(new InputMovable()); // Spécifique au player

        // Création d'un Ennemi
        var enemy = EntityFactory.CreateActor(engine, new Vector2(100,100), "Assets/Sprites/enemy.png", "Slime", new List<string> {"Enemy"});
        enemy.AddBehavior(new Oscillator()); // Spécifique à l'ennemi
        
        engine.CurrentScene.AddEntity(player);
        engine.CurrentScene.AddEntity(enemy);
        
        engine.Run();
    }
}