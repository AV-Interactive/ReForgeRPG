    using Raylib_cs;
    using rlImGui_cs;
    using ImGuiNET;
    using ReForge.Engine.Core;
    using System.Numerics;
    using Reforge.Editor.Tools;
    using Reforge.Editor.UI;
    using ReForge.Engin.Core;
    using ReForge.Engine.World;

    namespace Reforge.Editor.Core;

    public class EditorApp
    {
        const int _appWidth = 1280;
        const int _appHeight = 720;
        Engine _engine;
        bool _running = true;
        int _currentLayer = 0;
        
        public enum EditorState {Editing, Playing};
        EditorState _currentState = EditorState.Editing;
        
        MapPainter _mapPainter = new MapPainter();
        ViewportPanel _viewportPanel = new ViewportPanel();
        ContentBrowser _contentBrowser = new ContentBrowser();
        HierarchyPanel _hierarchyPanel = new HierarchyPanel();
        LayerPanel _layerPanel = new LayerPanel();
        InspectorPanel _inspectorPanel = new InspectorPanel();
        HighlighCellGizmo _gizmoHighlighCell = new HighlighCellGizmo();
        EditorSelector _editorSelector = new EditorSelector();
        MenuBarPanel _menuBar = new MenuBarPanel();

        public EditorApp()
        {
            _engine = new Engine(_appWidth, _appHeight, "ReForge Editor");
            if (ProjectManager.TryLoadLastProject())
            {
                Console.WriteLine($"Projet chargé : {ProjectManager.CurrentProject.ProjectName}");
            }
            else
            {
                Console.WriteLine($"Aucun projet précédent trouvé ou erreur lors du chargement.");
                ProjectManager.CreateEmptyTemporaryProject();
            }
            _engine.Initialize();
            EditorConfig.CurrentTool = EditorTool.Drawing;
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
            
            // Calcule du pourcentage pour l'inspecteur
            float windowWidth = Raylib.GetScreenWidth();
            float sidebarWidth = Math.Max(windowWidth * 0.15f, 200f);   // 15% ou 200px min
            float inspectorWidth = Math.Max(windowWidth * 0.20f, 300f);
            
            // Assignation du contexte
            var ctx = new EditorContext
            {
                State = _currentState,
                CurrentLayer = _currentLayer,
                MapPainter = _mapPainter,
                Hierarchy = _hierarchyPanel,
                Gizmo = _gizmoHighlighCell,
                ContentBrowser = _contentBrowser,
                SidebarWidth = sidebarWidth, 
                InspectorWidth = inspectorWidth,
                MenuBarHeight = ImGui.GetFrameHeightWithSpacing()
            };
            
            // Affichage des panneaux
            _menuBar.Draw(_engine, ctx);
            _hierarchyPanel.Draw(_engine.CurrentScene.Entities, ctx);
            _contentBrowser.Draw(_engine, ctx);
            _layerPanel.Draw(ctx);
            _viewportPanel.Draw(_engine, ctx, this);
            _inspectorPanel.Draw(_hierarchyPanel.SelectedEntity, ctx);
            
            _currentLayer = ctx.CurrentLayer;
        }

        public void HandleEditorTools(Vector2 viewportPos)
        {
            Vector2 mousePos = ImGui.GetMousePos();
            Vector2 relativeMousePos = mousePos - viewportPos;
            
            if (EditorConfig.CurrentTool == EditorTool.Drawing)
            {
                // Logique de dessin
                if (!string.IsNullOrEmpty(_contentBrowser.SelectedAsset))
                {
                    _mapPainter.Update(_engine, _contentBrowser.SelectedAsset, _currentLayer, relativeMousePos);
                }
            } 
            else if (EditorConfig.CurrentTool == EditorTool.Selection)
            {
                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                {
                    Vector2 snappedPos = EditorMath.SnapToGridRelativePos(relativeMousePos);
                    Entity entity = _editorSelector.GetEntityAt(_engine.CurrentScene, snappedPos, _currentLayer);
                    _hierarchyPanel.SelectedEntity = entity;
                }
                if (_hierarchyPanel.SelectedEntity != null)
                {
                    _gizmoHighlighCell.Update(_hierarchyPanel.SelectedEntity, viewportPos);
                }
            }
        }
        
        void Cleanup()
        {
            _engine.CleanUp();
        }
    }
