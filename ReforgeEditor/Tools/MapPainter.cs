using System.Numerics;
using ImGuiNET;
using Raylib_cs;
using Reforge.Editor.Core;
using ReForge.Engin.Core;
using ReForge.Engine.Core;
using ReForge.Engine.World;

namespace Reforge.Editor.Tools;

public class MapPainter
{
    int GetTileSize()
    {
        return ProjectManager.CurrentProject?.TileSize > 0 
            ? ProjectManager.CurrentProject.TileSize 
            : (int)EditorConfig.TileSize;
    }

    Vector2 _lastSnappedPos;
    bool _hasPreview;
    string _lastAsset = string.Empty;
    Vector2 _startRectPos;
    Vector2 _currentRectPos;
    bool _isDrawingRect;

    public void Update(Engine engine, EditorContext ctx, string selectedAsset, int currentLayerFromEditor, Vector2 relativeMousePos)
    {
        if (string.IsNullOrEmpty(selectedAsset))
        {
            _hasPreview = false;
            _lastAsset = string.Empty;
            return;
        }
        
        Vector2 snappedPos = EditorMath.SnapToGrid(relativeMousePos);
        
        _lastSnappedPos = snappedPos;
        _hasPreview = ImGui.IsWindowHovered();
        _lastAsset = selectedAsset;

        string fullAssetPath = selectedAsset;
        if (!Path.IsPathRooted(selectedAsset))
        {
            fullAssetPath = Path.Combine(ProjectManager.ProjectRootPath, ProjectManager.CurrentProject.AssetDirectory, selectedAsset);
        }

        if (EditorConfig.CurrentPaintingMode == PaintingMode.Brush)
        {
            if (_hasPreview && ImGui.IsMouseDown(ImGuiMouseButton.Left))
            {
                PlaceEntityAt(engine, ctx, selectedAsset, snappedPos, currentLayerFromEditor);
            }
        } 
        else if (EditorConfig.CurrentPaintingMode == PaintingMode.Rectangle)
        {
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) && ImGui.IsWindowHovered())
            {
                _startRectPos = snappedPos;
                _isDrawingRect = true;
            }

            if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
            {
                _currentRectPos = snappedPos;
            }

            if (_isDrawingRect && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
            {
                _isDrawingRect = false;
                float minX = MathF.Min(_startRectPos.X, _currentRectPos.X);
                float maxX = MathF.Max(_startRectPos.X, _currentRectPos.X);
                float minY = MathF.Min(_startRectPos.Y, _currentRectPos.Y);
                float maxY = MathF.Max(_startRectPos.Y, _currentRectPos.Y);

                int tileSize = GetTileSize();
                for (float x = minX; x <= maxX; x += tileSize)
                {
                    for (float y = minY; y <= maxY; y += tileSize)
                    {
                        PlaceEntityAt(engine, ctx, selectedAsset, new Vector2(x, y), currentLayerFromEditor);
                    }
                }
            }
        }
        else if (EditorConfig.CurrentPaintingMode == PaintingMode.Eraser)
        {
            if (_hasPreview && ImGui.IsMouseDown(ImGuiMouseButton.Left))
            {
                DeleteEntityAt(engine, snappedPos, currentLayerFromEditor);
            }
        }
    }

    void PlaceEntityAt(Engine engine, EditorContext ctx, string selectedAsset, Vector2 position, int layer)
    {
        int tileSize = GetTileSize();

        // Si on est dans le dossier Actors, on crée une Entity individuelle au lieu d'une Tilemap
        if (selectedAsset.StartsWith("Actors"))
        {
            string fullPath = selectedAsset;
            if (!Path.IsPathRooted(selectedAsset))
            {
                fullPath = Path.Combine(ProjectManager.ProjectRootPath, ProjectManager.CurrentProject.AssetDirectory, selectedAsset);
            }

            // On vérifie si une entité existe déjà à cette position exacte sur ce layer
            var existing = engine.CurrentScene.Entities
                .FirstOrDefault(e => e.Position == position && e.ZIndex == layer);
            
            if (existing != null) return;

            var texture = engine.AssetManager.GetTexture(fullPath);
            var entity = new Entity(position, texture, Path.GetFileNameWithoutExtension(selectedAsset), selectedAsset);
            entity.ZIndex = layer;
            engine.CurrentScene.AddEntity(entity);
            return;
        }

        var tilemap = engine.CurrentScene.Entities
            .OfType<Tilemap>()
            .FirstOrDefault(t => t.ZIndex == layer);

        if (tilemap == null)
        {
            string fullPath = selectedAsset;
            if (!Path.IsPathRooted(selectedAsset))
            {
                fullPath = Path.Combine(ProjectManager.ProjectRootPath, ProjectManager.CurrentProject.AssetDirectory, selectedAsset);
            }

            var texture = engine.AssetManager.GetTexture(fullPath);
            tilemap = new Tilemap(Vector2.Zero, tileSize, texture, selectedAsset);
            tilemap.Name = $"Tilemap [couche {layer}]";
            tilemap.ZIndex = layer;
            
            tilemap.AddLayer(new TileLayer(5, 5));
            
            engine.CurrentScene.AddEntity(tilemap);
        }
        
        int gridX = (int)(position.X / tileSize);
        int gridY = (int)(position.Y / tileSize);

        var firstLayer = tilemap.GetLayers()[0];

        if (gridX >= 0 && gridY >= 0) 
        {
            int currentH = firstLayer.Data.Length;
            int currentW = firstLayer.Data[0].Length;
    
            if (gridX >= currentW || gridY >= currentH)
            {
                int newW = Math.Max(currentW, gridX + 1);
                int newH = Math.Max(currentH, gridY + 1);
                firstLayer.Resize(newW, newH);
            }
    
            // On stocke les tuiles en 1-based dans la tilemap (0 = vide)
            firstLayer.Data[gridY][gridX] = ctx.SelectedTile + 1;
        }
    }

    void DeleteEntityAt(Engine engine, Vector2 position, int layer)
    {
        int tileSize = GetTileSize();
        var tilemap = engine.CurrentScene.Entities.OfType<Tilemap>().FirstOrDefault(t => t.ZIndex == layer);

        if (tilemap == null) return;
        
        int gridX = (int)(position.X / tileSize);
        int gridY = (int)(position.Y / tileSize);

        var firstLayer = tilemap.GetLayers()[0];
        if (gridY >= 0 && gridY < firstLayer.Data.Length && gridX >= 0 && gridX < firstLayer.Data[0].Length)
        {
            firstLayer.Data[gridY][gridX] = 0;
        }
    }
    
    public void DrawPreview(Engine engine, EditorContext ctx)
    {
        if (!_hasPreview || string.IsNullOrEmpty(_lastAsset)) return;
    
        string fullPath = _lastAsset;
        if (!Path.IsPathRooted(_lastAsset))
        {
            fullPath = Path.Combine(ProjectManager.ProjectRootPath, ProjectManager.CurrentProject.AssetDirectory, _lastAsset);
        }

        Texture2D tex = engine.AssetManager.GetTexture(fullPath);
        float tileSize = GetTileSize();
        
        int cols = tex.Width / (int)tileSize;
        int x = ctx.SelectedTile % cols;
        int y = ctx.SelectedTile / cols;

        Rectangle sourceRec = new Rectangle(
            x * tileSize, 
            y * tileSize, 
            tileSize, 
            tileSize
        );
    
        // On dessine la tuile fantôme (semi-transparente)
        Raylib.DrawTextureRec(tex, sourceRec, _lastSnappedPos, Raylib.Fade(Color.White, 0.5f));
    }
    
    public void DrawGrid(RenderTexture2D viewport)
    {
        int tileSize = GetTileSize();
        Color gridColor = new Color(50, 50, 50, 255);
        int width = viewport.Texture.Width;
        int height = viewport.Texture.Height;

        for (int x = 0; x <= width; x += tileSize)
        {
            Raylib.DrawLine(x, 0, x, height, gridColor);
        }

        for (int y = 0; y <= height; y += tileSize)
        {
            Raylib.DrawLine(0, y, width, y, gridColor);
        }
    }
}
