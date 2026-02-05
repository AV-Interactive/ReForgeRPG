using Raylib_cs;
using ReForge.Engin.Physics;
using ReForge.Engine.World;
namespace ReForge.Engine.Core;

public class Engine
{
    int _width;

    int _height;

    string _winName;

    public Engine(int whidth, int height, string winName)
    {
        _width = whidth;
        _height = height;
        _winName = winName;
    }
    
    public void Initialize()
    {
        Raylib.InitWindow(_width, _height, _winName);
        Raylib.SetTargetFPS(60);
    }

    public void Run()
    {
        while (!Raylib.WindowShouldClose())
        {
            float deltaTime = Raylib.GetFrameTime();

            CurrentScene.Update(deltaTime);
            
            Raylib.BeginDrawing();
            
            Raylib.ClearBackground(Color.Black);
            
            CurrentScene.Draw();
            
            Raylib.EndDrawing();
        }
        Raylib.CloseWindow();
        AssetManager.UnloadAll();
    }
    
    public AssetManager AssetManager { get; set; } = new AssetManager();
    
    public Texture2D LoadTexture(string path) => AssetManager.GetTexture(path);
    
    public Scene CurrentScene { get; set; } = new Scene();
    
    public void DestroyEntity(Entity entity) => CurrentScene.DestroyEntity(entity);
}
