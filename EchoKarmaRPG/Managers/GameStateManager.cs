using System.Numerics;
using ReForge.Engin.Core;
using ReForge.Engine.Core;
using ReForge.Engine.World;

namespace EchoKarmaRPG.Managers;

public class GameStateManager
{
    public static string CurrentState { get; set; }
    public static Entity Player { get; set; }
    public static int PlayerHealth { get; set; } = 100;
    
    public static HashSet<string> WorldState = new HashSet<string>();
    public static List<string> Inventory = new List<string>();
    
    public static void TransitionToScene(Engine engine, string sceneName, Vector2 newPosition)
    {
        string nextScenePath = Path.Combine(ProjectManager.CurrentProject.SceneDirectory, sceneName + ".scn");
        SceneSerializer.Load(engine.CurrentScene, engine, nextScenePath);
        
        Player = engine.CurrentScene.Entities.FirstOrDefault(e => e.HasTag("Player"));
        
        if (Player != null)
        {
            Player.Position = newPosition;
        }
    }
}
