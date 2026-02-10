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
        int _selectedTile = 0;
        
        public enum EditorState {Editing, Playing};
        EditorState _currentState = EditorState.Editing;
        public List<Entity> _snapshotEntities = new List<Entity>();
        public List<Entity> _selectedEntities = new List<Entity>();
        
        MapPainter _mapPainter = new MapPainter();
        ViewportPanel _viewportPanel = new ViewportPanel();
        ContentBrowser _contentBrowser = new ContentBrowser();
        HierarchyPanel _hierarchyPanel = new HierarchyPanel();
        LayerPanel _layerPanel = new LayerPanel();
        InspectorPanel _inspectorPanel = new InspectorPanel();
        HighlighCellGizmo _gizmoHighlighCell = new HighlighCellGizmo();
        EditorSelector _editorSelector = new EditorSelector();
        MenuBarPanel _menuBar = new MenuBarPanel();
        
        EditorContext _ctx = new EditorContext();

        public EditorApp()
        {
            _engine = new Engine(_appWidth, _appHeight, "ReForge Editor");
            _engine.Initialize();
            if (ProjectManager.TryLoadLastProject())
            {
                if (!string.IsNullOrEmpty(ProjectManager.CurrentProject.LastScenePath))
                {
                    string fullScenePath = Path.Combine(ProjectManager.ProjectRootPath,
                        ProjectManager.CurrentProject.LastScenePath);
                    if (File.Exists(fullScenePath))
                    {
                        SceneSerializer.Load(_engine.CurrentScene, _engine, fullScenePath);
                        ProjectManager.CurrentSceneName = Path.GetFileNameWithoutExtension(fullScenePath);
                        ProjectManager.CurrentScene = _engine.CurrentScene;
                        Console.WriteLine("Dernière scène restaurée avec succès");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Aucun projet précédent trouvé ou erreur lors du chargement.");
                ProjectManager.CreateEmptyTemporaryProject();
            }

            // Synchroniser la taille des tuiles de l'éditeur avec le projet chargé
            if (ProjectManager.CurrentProject != null && ProjectManager.CurrentProject.TileSize > 0)
            {
                EditorConfig.TileSize = ProjectManager.CurrentProject.TileSize;
            }

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
            _ctx = new EditorContext
            {
                State = _currentState,
                SnapshotEntities = _snapshotEntities,
                SelectedEntities = _selectedEntities,
                EditorSelector = _editorSelector,
                CurrentLayer = _currentLayer,
                MapPainter = _mapPainter,
                Hierarchy = _hierarchyPanel,
                Gizmo = _gizmoHighlighCell,
                ContentBrowser = _contentBrowser,
                SidebarWidth = sidebarWidth, 
                InspectorWidth = inspectorWidth,
                MenuBarHeight = ImGui.GetFrameHeightWithSpacing(),
                CurrentScene = _engine.CurrentScene,
                SelectedTile = _selectedTile
            };
            
            // Affichage des panneaux
            _menuBar.Draw(_engine, _ctx);
            _hierarchyPanel.Draw(_engine.CurrentScene.Entities, _ctx);
            _contentBrowser.Draw(_engine, _ctx);
            _layerPanel.Draw(_ctx);
            _viewportPanel.Draw(_engine, _ctx, this);
            _gizmoHighlighCell.UpdateTimer();
            _inspectorPanel.Draw(_ctx.SelectedEntities.FirstOrDefault(), _ctx);

            _currentState = _ctx.State;
            _currentLayer = _ctx.CurrentLayer;
            _selectedTile = _ctx.SelectedTile;
        }

        public void HandleEditorTools(Vector2 viewportPos)
        {
            Vector2 mousePos = ImGui.GetMousePos();
            Vector2 relativeMousePos = mousePos - viewportPos;
            Vector2 snappedPos = EditorMath.SnapToGridRelativePos(relativeMousePos);

            bool isHovered = ImGui.IsWindowHovered();

            if (EditorConfig.CurrentTool == EditorTool.Drawing)
            {
                if (!isHovered) return;
                
                // Logique de dessin
                if (!string.IsNullOrEmpty(_contentBrowser.SelectedAsset))
                {
                    _mapPainter.Update(_engine, _ctx, _contentBrowser.SelectedAsset, _currentLayer, relativeMousePos);
                }
            } 
            
            else if (EditorConfig.CurrentTool == EditorTool.Selection)
            {
                if (isHovered && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                {
                    // Pour la sélection, on préfère utiliser la position réelle de la souris dans le monde 
                    // plutôt qu'une position snappée, pour permettre de cliquer n'importe où sur un grand sprite.
                    Entity entity = _editorSelector.GetEntityAt(_engine.CurrentScene, relativeMousePos, _currentLayer);

                    if (ImGui.GetIO().KeyCtrl)
                    {
                        if (entity != null)
                        {
                            if (_selectedEntities.Contains(entity)) _selectedEntities.Remove(entity);
                            else _selectedEntities.Add(entity);
                        }
                    }
                    else
                    {
                        _selectedEntities.Clear();
                        if (entity != null) _selectedEntities.Add(entity);
                    }
                }

                if (isHovered && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                {
                    ImGui.OpenPopup("ViewportContextMenu");
                }
                
                if (ImGui.BeginPopup("ViewportContextMenu"))
                {
                    if (ImGui.MenuItem("Créer une entité vide"))
                    {
                        var emptyEntity = new Entity();
                        emptyEntity.Name = "Nouvelle entité";
                        emptyEntity.Position = snappedPos;
                        emptyEntity.ZIndex = _currentLayer;
                       
                        _engine.CurrentScene.AddEntity(emptyEntity);
                        
                        _selectedEntities.Clear();
                        _selectedEntities.Add(emptyEntity);
                    }

                    if (_selectedEntities.Count > 0)
                    {
                        ImGui.Separator();
                        if (ImGui.MenuItem("Supprimer la sélection"))
                        {
                            foreach (var entity in _selectedEntities)
                            {
                                _engine.CurrentScene.DestroyEntity(entity);
                            }
                            _selectedEntities.Clear();
                        }
                    }
                    ImGui.EndPopup();
                }
            }
        }
        
        void Cleanup()
        {
            _engine.CleanUp();
        }
    }
