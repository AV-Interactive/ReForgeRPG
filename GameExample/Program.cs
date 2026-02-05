using System.Numerics;
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
        
        var player = engine.LoadTexture("Assets/Sprites/player.png");
        var enemyTexture = engine.LoadTexture("Assets/Sprites/enemy.png");
        
        Entity playerEntity = new Entity(new Vector2(400, 300), player, "Player");
        Entity enemy = new Entity(new Vector2(100, 300), enemyTexture, "Enemy");
        
        // On ajoute le comportement de mouvement au joueur
        playerEntity.AddBehavior(new InputMovable { Speed = 250});
        
        // On ajoute le comportement d'oscillation a l'ennemi
        enemy.AddBehavior(new Oscillator
        {
            Direction = new Vector2(1, 0),
            Distance = 100f,
            Speed = 150f
        });
        
        engine.AddEntity(playerEntity);
        engine.AddEntity(enemy);
        
        engine.Run();
    }
}