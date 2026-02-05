using Raylib_cs;
using ReForge.Engine.World;
namespace ReForge.Engine.Core;

public class Engine
{
    int _whidth;

    int _height;

    string _winName;

    List<Entity> _entities = new List<Entity>();

    public Engine(int whidth, int height, string winName)
    {
        _whidth = whidth;
        _height = height;
        _winName = winName;
    }

    public void AddEntity(Entity entity)
    {
        _entities.Add(entity);
    }

    /*
       L'ordre logique (La boucle de fer) :
       - Calculer le DeltaTime : On regarde combien de temps s'est écoulé.
       - Vérifier si une touche est pressée (Input) : Qu'est-ce que l'utilisateur veut faire ?
       - Calculer la nouvelle position (Update) : On met à jour la physique.
       - Effacer l'écran (Clear Background) : On passe un coup d'éponge sur la frame précédente.
       - Dessiner le décor et le joueur (Draw) : On affiche le résultat tout propre.
     */
    public void Initialize()
    {
        Raylib.InitWindow(_whidth, _height, _winName);
        Raylib.SetTargetFPS(60);
    }

    public void Run()
    {
        while (!Raylib.WindowShouldClose())
        {
            float deltaTime = Raylib.GetFrameTime();

            foreach (Entity entity in _entities)
            {
                entity.Update(deltaTime);
            }
            
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            foreach (Entity entity in _entities)
            {
                entity.Draw();
            }
            
            Raylib.EndDrawing();
        }
        Raylib.CloseWindow();
    }

    public Texture2D LoadTexture(string path)
    {
        return Raylib.LoadTexture(path);
    }

    public void DrawTexture(Texture2D texture, int x, int y)
    {
        Raylib.DrawTexture(texture, x, y, Color.White);
    }
}
