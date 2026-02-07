using Raylib_cs;
using ReForge.Engine.Physics;
using ReForge.Engine.World;
namespace ReForge.Engine.Core;

public class Engine
{
    int _width;

    int _height;

    string _winName;

    public Engine(int width, int height, string winName)
    {
        _width = width;
        _height = height;
        _winName = winName;
    }
    
    public void Initialize()
    {
        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
        Raylib.InitWindow(_width, _height, _winName);
        Raylib.SetTargetFPS(60);
    }

    public void Run()
    {
        while (!Raylib.WindowShouldClose())
        {
            float deltaTime = Raylib.GetFrameTime();
        
            Update(deltaTime);
        
            Raylib.BeginDrawing();
            Render();
            Raylib.EndDrawing();
        }
    }

    public void Update(float deltaTime)
    {
        CurrentScene.Update(deltaTime);
    }

    public void Render()
    {
        Raylib.ClearBackground(Color.Black);
        CurrentScene.Draw();
    }

    public void CleanUp()
    {
        Raylib.CloseWindow();
        AssetManager.UnloadAll();
    }
    
    public AssetManager AssetManager { get; set; } = new AssetManager();
    
    public Texture2D LoadTexture(string path) => AssetManager.GetTexture(path);
    
    public Scene CurrentScene { get; set; } = new Scene();
    
    public void DestroyEntity(Entity entity) => CurrentScene.DestroyEntity(entity);
}
