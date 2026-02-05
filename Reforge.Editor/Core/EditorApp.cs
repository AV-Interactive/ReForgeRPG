using Raylib_cs;
using rlImGui_cs;
using ImGuiNET;
using ReForge.Engine.Core;
using System.Numerics;
using Reforge.Editor.Tools;
using Reforge.Editor.UI;
using ReForge.Engine.World;

namespace Reforge.Editor.Core;

public class EditorApp
{
    Engine _engine;
    RenderTexture2D _viewportRes;
    bool _running = true;
    
    ContentBrowser _contentBrowser = new ContentBrowser();
    MapPainter _mapPainter = new MapPainter();

    public EditorApp()
    {
        _engine = new Engine(1280, 720, "ReForge Editor");
        _engine.Initialize();

        _viewportRes = Raylib.LoadRenderTexture(1280, 720);
    }

    public void Run()
    {
        rlImGui.Setup();
        while(_running && !Raylib.WindowShouldClose())
        {
            float deltaTime = Raylib.GetFrameTime();
            
            _engine.Update(deltaTime);
            
            Raylib.BeginTextureMode(_viewportRes);
            _engine.Render();
            
            _mapPainter.DrawPreview(_engine, _contentBrowser.SelectedAsset);
            _mapPainter.DrawGrid(); 
            Raylib.EndTextureMode();
            
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.DarkGray);
            
            rlImGui.Begin();

            DrawUI();
            
            rlImGui.End();
            
            Raylib.EndDrawing();
        }
        rlImGui.Shutdown();
        Cleanup();
    }
    
    void DrawUI()
    {
        ImGui.Begin("Game View");
        
        _contentBrowser.Draw();
        
        _mapPainter.Update(_engine, _contentBrowser.SelectedAsset);

        IntPtr texPtr = (IntPtr)_viewportRes.Texture.Id;
        ImGui.Image(texPtr, new Vector2(1280, 720), new Vector2(0, 1), new Vector2(1, 0));
        ImGui.End();

        ImGui.Begin("Hierarchy");
        foreach (Entity entity in _engine.CurrentScene.Entities)
        {
            if (ImGui.Selectable(entity.Name)) ;
        }
        ImGui.End();
    }
    
    void Cleanup()
    {
        Raylib.UnloadRenderTexture(_viewportRes);
        _engine.CleanUp();
    }
    
}
