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
        HierarchyPanel _hierarchyPanel = new HierarchyPanel();
        InspectorPanel _inspectorPanel = new InspectorPanel();
        TransformGizmo _gizmoTransform = new TransformGizmo();

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

                    if (_hierarchyPanel.SelectedEntity != null)
                    {
                        _gizmoTransform.Draw(_hierarchyPanel.SelectedEntity);
                    }
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

            // Affichage des panneaux
            _contentBrowser.Draw(_engine);
            _hierarchyPanel.Draw(_engine.CurrentScene.Entities);
            _inspectorPanel.Draw(_hierarchyPanel.SelectedEntity);
            DrawLayerControl();

            // Fenêtre de visualisation du jeu
            ImGui.Begin("Game View");
            
            Vector2 viewportPos = ImGui.GetCursorScreenPos();
            HandleViewportResizing();

            Vector2 regionMax = ImGui.GetContentRegionAvail();
            ImGui.Image((IntPtr)_viewportRes.Texture.Id, regionMax, new Vector2(0, 1), new Vector2(1, 0));

            // Logique des outils (Priorité au Gizmo sur le Pinceau)
            HandleEditorTools(viewportPos);

            ImGui.End();
        }

        void HandleViewportResizing()
        {
            Vector2 regionMax = ImGui.GetContentRegionAvail();
            if (regionMax.X != _viewportRes.Texture.Width || regionMax.Y != _viewportRes.Texture.Height)
            {
                if (regionMax.X > 0 && regionMax.Y > 0)
                {
                    Raylib.UnloadRenderTexture(_viewportRes);
                    _viewportRes = Raylib.LoadRenderTexture((int)regionMax.X, (int)regionMax.Y);
                }
            }
        }

        void HandleEditorTools(Vector2 viewportPos)
        {
            if (_hierarchyPanel.SelectedEntity != null)
            {
                _gizmoTransform.Update(_hierarchyPanel.SelectedEntity, viewportPos);
            }

            else if (!string.IsNullOrEmpty(_contentBrowser.SelectedAsset))
            {
                _mapPainter.Update(_engine, _contentBrowser.SelectedAsset, _currentLayer, viewportPos);
            }
            else
            {
                ImGui.SetCursorPos(new Vector2(10, 30));
                ImGui.TextColored(new Vector4(1, 1, 0, 1), "Mode Selection: Cliquez sur une entité dans la hiérarchie.");
            }
        }

        void DrawLayerControl()
        {
            ImGui.Begin("Layer Control");
            ImGui.RadioButton("Background", ref _currentLayer, 0);
            ImGui.RadioButton("World", ref _currentLayer, 1);
            ImGui.RadioButton("Foreground", ref _currentLayer, 2);
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
                if (ImGui.BeginMenu("Fichier"))
                {
                    if (ImGui.MenuItem("Sauvegarder la Scène"))
                    {
                        string fullPath = Path.Combine(ProjectPaths.Scenes, "01.json");
                        SceneSerializer.Save(_engine.CurrentScene, fullPath);
                    }
                    if (ImGui.MenuItem("Quitter")) _running = false;
                
                    ImGui.EndMenu();
                }
                
                ImGui.Separator();
                
                
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
