using Raylib_cs;
using ReForge.Engine.World;

namespace ReForge.Engine.Core;

public class Engine
{
    int _width;

    int _height;

    string _winName;
    
    public static Engine Instance { get; private set; } = null!;

    public Engine(int width, int height, string winName)
    {
        _width = width;
        _height = height;
        _winName = winName;

        Instance = this;
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
        
        var camEntity = CurrentScene.GetActiveCameraEntity();

        if (camEntity != null)
        {
            Camera2D worldCamera = new Camera2D
            {
                Target = camEntity.Position,
                Offset = new System.Numerics.Vector2(_width / 2, _height / 2),
                Zoom = 1.0f,
                Rotation = 0.0f,
            };
            
            Raylib.BeginMode2D(worldCamera);
            CurrentScene.Draw();
            Raylib.EndMode2D();
        }
        else
        {
            CurrentScene.Draw();
        }
        
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
