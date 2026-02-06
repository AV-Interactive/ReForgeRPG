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
        const int _appWidth = 1280;
        const int _appHeight = 720;
        Engine _engine;
        RenderTexture2D _viewportRes;
        bool _running = true;
        int _currentLayer = 0;
        
        public enum EditorState {Editing, Playing};
        EditorState _currentState = EditorState.Editing;
        
        ContentBrowser _contentBrowser = new ContentBrowser();
        MapPainter _mapPainter = new MapPainter();

        public EditorApp()
        {
            _engine = new Engine(_appWidth, _appHeight, "ReForge Editor");
            _engine.Initialize();

            _viewportRes = Raylib.LoadRenderTexture(_appWidth, _appHeight);
        }

        public void Run()
        {
            
            rlImGui.Setup();
            while(_running && !Raylib.WindowShouldClose())
            {
                float deltaTime = Raylib.GetFrameTime();

                if (_currentState == EditorState.Playing)
                {
                    _engine.Update(deltaTime);
                }
        
                Raylib.BeginTextureMode(_viewportRes);
                _engine.Render();

                if (_currentState == EditorState.Editing)
                {
                    _mapPainter.DrawPreview(_engine);
                    _mapPainter.DrawGrid(_viewportRes); 
                }
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
            ImGui.DockSpaceOverViewport(0, ImGui.GetMainViewport());
            
            DrawMenuBar();
            
            _contentBrowser.Draw();

            if (!string.IsNullOrEmpty(_contentBrowser.SelectedAsset))
            {
                ImGui.Begin("Game View");

                _engine.AssetManager.GetTexture(_contentBrowser.SelectedAsset);

                Vector2 screenPos = ImGui.GetCursorScreenPos();
                _mapPainter.Update(_engine, _contentBrowser.SelectedAsset, _currentLayer, screenPos);
                Vector2 regionMax = ImGui.GetContentRegionAvail();
                
                if (regionMax.X != _viewportRes.Texture.Width || regionMax.Y != _viewportRes.Texture.Height)
                {
                    if (regionMax.X > 0 && regionMax.Y > 0) 
                    {
                        Raylib.UnloadRenderTexture(_viewportRes);
                        _viewportRes = Raylib.LoadRenderTexture((int)regionMax.X, (int)regionMax.Y);
                    }
                }
                
                ImGui.Image((IntPtr)_viewportRes.Texture.Id, regionMax, new Vector2(0, 1), new Vector2(1, 0));
            
                ImGui.End();
            }
            else
            {
                ImGui.Begin("Game View");
                ImGui.Text("Sélectionnez un asset pour commencer à peindre");
                ImGui.End();
            }

            ImGui.Begin("Layer Control");
            ImGui.RadioButton("Backgound", ref _currentLayer, 0);
            ImGui.RadioButton("World", ref _currentLayer, 1);
            ImGui.RadioButton("Foreground", ref _currentLayer, 2);
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
        
        void DrawMenuBar()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (_currentState == EditorState.Editing)
                {
                    if (ImGui.MenuItem("Play")) _currentState = EditorState.Playing;
                }
                else
                {
                    if (ImGui.MenuItem("Stop")) _currentState = EditorState.Editing;
                }
                ImGui.EndMainMenuBar();
            }
        }
    }
