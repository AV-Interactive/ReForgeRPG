using System.Numerics;
using ImGuiNET;
using Raylib_cs;
using Reforge.Editor.Core;
using ReForge.Engine.Core;
using ReForge.Engine.World;

namespace Reforge.Editor.Tools;

public class MapPainter
{
    public float GridSize = 32.0f;

    Vector2 _lastSnappedPos;
    bool _hasPreview;
    string _lastAsset = string.Empty;
    Vector2 _startRectPos;
    Vector2 _currentRectPos;
    bool _isDrawingRect;

    public void Update(Engine engine, string selectedAsset, int currentLayerFromEditor, Vector2 relativeMousePos)
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

        if (EditorConfig.CurrentPaintingMode == PaintingMode.Brush)
        {
            if (_hasPreview && ImGui.IsMouseDown(ImGuiMouseButton.Left))
            {
                PlaceEntityAt(engine, selectedAsset, snappedPos, currentLayerFromEditor);
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

                for (float x = minX; x <= maxX; x += GridSize)
                {
                    for (float y = minY; y <= maxY; y += GridSize)
                    {
                        PlaceEntityAt(engine, selectedAsset, new Vector2(x, y), currentLayerFromEditor);
                    }
                }
            }
        }
        else if (EditorConfig.CurrentPaintingMode == PaintingMode.Eraser)
        {
            if (_hasPreview && ImGui.IsMouseDown(ImGuiMouseButton.Left))
            {
                DeleteEntityAt(engine, selectedAsset, snappedPos, currentLayerFromEditor);
            }
        }
    }

    void PlaceEntityAt(Engine engine, string selectedAsset, Vector2 position, int layer)
    {
        int targetX = (int)MathF.Round(position.X);
        int targetY = (int)MathF.Round(position.Y);
        
        var existingTile = engine.CurrentScene.Entities.FirstOrDefault(e => 
            (int)MathF.Round(e.Position.X) == targetX && 
            (int)MathF.Round(e.Position.Y) == targetY && 
            e.ZIndex == layer);

        if (existingTile != null)
        {
            if (existingTile.TexturePath == selectedAsset) return;
            
            engine.CurrentScene.DestroyEntity(existingTile);
        }
        
        var tex = engine.AssetManager.GetTexture(selectedAsset);
        var entity = new Entity(position, tex, "NewTile", selectedAsset);
        entity.Name = selectedAsset.Split('/').Last().Split('.').First();
        entity.ZIndex = layer;
        engine.CurrentScene.AddEntity(entity);
    }

    void DeleteEntityAt(Engine engine, string selectedAsset, Vector2 position, int layer)
    {
        int targetX = (int)MathF.Round(position.X);
        int targetY = (int)MathF.Round(position.Y);
        
        var existingTile = engine.CurrentScene.Entities.FirstOrDefault(e => 
            (int)MathF.Round(e.Position.X) == targetX && 
            (int)MathF.Round(e.Position.Y) == targetY && 
            e.ZIndex == layer && e.TexturePath == selectedAsset);

        if (existingTile != null)
        {
            engine.CurrentScene.DestroyEntity(existingTile);
        }
    }
    
    public void DrawPreview(Engine engine)
    {
        if (!_hasPreview || string.IsNullOrEmpty(_lastAsset)) return;
        var tex = engine.AssetManager.GetTexture(_lastAsset);

        if (EditorConfig.CurrentPaintingMode == PaintingMode.Brush)
        {
            Raylib.DrawTextureV(tex, _lastSnappedPos, Raylib.Fade(Color.White, 0.5f));
        }
        else if (EditorConfig.CurrentPaintingMode == PaintingMode.Rectangle && ImGui.IsMouseDown(ImGuiMouseButton.Left))
        {
            float minX = MathF.Min(_startRectPos.X, _currentRectPos.X);
            float maxX = MathF.Max(_startRectPos.X, _currentRectPos.X);
            float minY = MathF.Min(_startRectPos.Y, _currentRectPos.Y);
            float maxY = MathF.Max(_startRectPos.Y, _currentRectPos.Y);

            for (float x = minX; x <= maxX; x += GridSize)
            {
                for (float y = minY; y <= maxY; y += GridSize)
                {
                    Raylib.DrawTextureV(tex, new Vector2(x, y), Raylib.Fade(Color.White, 0.5f));
                }
            }
        }
    }
    
    public void DrawGrid(RenderTexture2D viewport)
    {
        Color gridColor = new Color(50, 50, 50, 255);
        int width = viewport.Texture.Width;
        int height = viewport.Texture.Height;

        for (int x = 0; x <= width; x += (int)GridSize)
        {
            Raylib.DrawLine(x, 0, x, height, gridColor);
        }

        for (int y = 0; y <= height; y += (int)GridSize)
        {
            Raylib.DrawLine(0, y, width, y, gridColor);
        }
    }
}
