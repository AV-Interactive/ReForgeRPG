using System.Numerics;
using ReForge.Engine.Core;
using ReForge.Engine.World;

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
        
        engine.AddEntity(playerEntity);
        engine.AddEntity(enemy);
        
        engine.Run();
    }
}