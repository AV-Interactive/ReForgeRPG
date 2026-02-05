using Raylib_cs;
namespace ReForge.Engine.Core;

public class Engine
{
    /* Paramètres des fenêtres */
    int _whidth;

    int _height;

    string _winName;

    public Engine(int whidth, int height, string winName)
    {
        _whidth = whidth;
        _height = height;
        _winName = winName;
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
            Raylib.BeginDrawing();
            Raylib.EndDrawing();
        }
        Raylib.CloseWindow();
    }
}
