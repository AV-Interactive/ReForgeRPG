using System.Numerics;
using GameExample.Factories;
using ReForge.Engine.Core;
using ReForge.Engine.World;
using ReForge.Engine.World.Behaviors;

namespace GameExample;

public static class EveilDuGardien
{
    public static void Setup(Engine engine)
    {
        // Salle 1 : Exploration (0, 0)
        // Joueur
        var player = EntityFactory.CreateActor(engine, new Vector2(100, 100), "Assets/Sprites/player.png", "Gardien", new List<string> { "Player" });
        player.AddBehavior(new InputMovable { Speed = 200f });
        engine.CurrentScene.AddEntity(player);

        // Décor de la Salle 1 (MapPainter simulation)
        for (int i = 0; i < 10; i++)
        {
            var wall = new Entity(new Vector2(i * 32, 0), engine.LoadTexture("Assets/Sprites/enemy.png"), "Wall", "Assets/Sprites/enemy.png");
            wall.ZIndex = 0;
            wall.AddBehavior(new BoxCollider { Width = 32, Height = 32 });
            engine.CurrentScene.AddEntity(wall);
        }

        // Salle 2 : Énigme (320, 0)
        // Pièges mobiles
        for (int i = 0; i < 3; i++)
        {
            var trap = EntityFactory.CreateActor(engine, new Vector2(400, 100 + i * 100), "Assets/Sprites/enemy.png", "Trap", new List<string> { "Trap" });
            trap.AddBehavior(new Oscillator { Direction = new Vector2(0, 1), Distance = 50, Speed = 2f });
            engine.CurrentScene.AddEntity(trap);
        }

        // Salle 3 : Interaction (640, 0)
        // Levier (Trigger)
        var lever = new Entity(new Vector2(700, 300), engine.LoadTexture("Assets/Sprites/player.png"), "Levier", "Assets/Sprites/player.png");
        var leverCollider = new BoxCollider { Width = 32, Height = 32, IsTrigger = true };
        lever.AddBehavior(leverCollider);
        
        var trigger = lever.GetBehavior<ActionTrigger>();
        if (trigger != null)
        {
            trigger.OnEnterActions.Add(new ActionCommand
            {
                Verb = ActionVerb.Destroy,
                TargetTag = "Porte"
            });
            trigger.OnEnterActions.Add(new ActionCommand
            {
                Verb = ActionVerb.Destroy,
                TargetSelf = true
            });
        }
        engine.CurrentScene.AddEntity(lever);

        // Porte (à détruire par le levier)
        var door = new Entity(new Vector2(750, 300), engine.LoadTexture("Assets/Sprites/enemy.png"), "Porte", "Assets/Sprites/enemy.png");
        door.AddTag("Porte");
        door.AddBehavior(new BoxCollider { Width = 32, Height = 32 });
        engine.CurrentScene.AddEntity(door);
    }
}
