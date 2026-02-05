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
        
        var playerTexture = engine.LoadTexture("Assets/Sprites/player.png");
        var enemyTexture = engine.LoadTexture("Assets/Sprites/enemy.png");
        
        Entity playerEntity = new Entity(new Vector2(400, 300), playerTexture, "Player");
        Entity enemyEntity = new Entity(new Vector2(250, 300), enemyTexture, "Enemy");
        Entity enemyDestroyableEntity = new Entity(new Vector2(30, 50), enemyTexture, "Enemy destroyable");
        
        // On ajoute le comportement de mouvement au joueur
        playerEntity.AddBehavior(new InputMovable { Speed = 250});
        
        // On ajoute le comportement d'oscillation a l'ennemi
        enemyEntity.AddBehavior(new Oscillator
        {
            Direction = new Vector2(1, 0),
            Distance = 150f,
            Speed = 150f
        });
        
        // On ajoute des hitboxes au joueur et l'ennemi 1 et 2
        var playerCollider = new BoxCollider();
        playerCollider.OnCollisionEnter = (other) =>
        {
            Console.WriteLine($"Toine, tu viens de toucher {other.Name} !");
        };

        var enemyDestroyable = new BoxCollider();
        enemyDestroyable.OnCollisionEnter = (other) =>
        {
            if (other.Name == "Player")
            {
                engine.DestroyEntity(enemyDestroyableEntity);
                Console.WriteLine("Enemy lose !");
            }
        };
        
        playerEntity.AddBehavior(playerCollider);
        enemyEntity.AddBehavior(new BoxCollider());
        enemyDestroyableEntity.AddBehavior(enemyDestroyable);
        
        engine.CurrentScene.AddEntity(playerEntity);
        engine.CurrentScene.AddEntity(enemyEntity);
        engine.CurrentScene.AddEntity(enemyDestroyableEntity);
        
        engine.Run();
    }
}