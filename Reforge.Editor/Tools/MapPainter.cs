using System.Numerics;
using ImGuiNET;
using Raylib_cs;
using ReForge.Engine.Core;
using ReForge.Engine.World;

namespace Reforge.Editor.Tools;

public class MapPainter
{
    public float GridSize = 32.0f;

    Vector2 _lastSnappedPos;
    bool _hasPreview;
    string _lastAsset = string.Empty;
    
    public void Update(Engine engine, string selectedAsset, int currentLayerFromEditor, Vector2 viewportScreenPos)
    {
        if (string.IsNullOrEmpty(selectedAsset))
        {
            _hasPreview = false;
            _lastAsset = string.Empty;
            return;
        }
        
        Vector2 mousePos = ImGui.GetMousePos();
        Vector2 relativeMousePos = mousePos - viewportScreenPos;

        Vector2 snappedPos = new Vector2(
            MathF.Floor(relativeMousePos.X / GridSize) * GridSize,
            MathF.Floor(relativeMousePos.Y / GridSize) * GridSize
        );
        
        _lastSnappedPos = snappedPos;
        _hasPreview = ImGui.IsWindowHovered();
        _lastAsset = selectedAsset;

        if (_hasPreview && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
        {
            int targetX = (int)MathF.Round(snappedPos.X);
            int targetY = (int)MathF.Round(snappedPos.Y);

            var existingTile = engine.CurrentScene.Entities.FirstOrDefault(e => 
                (int)MathF.Round(e.Position.X) == targetX && 
                (int)MathF.Round(e.Position.Y) == targetY && 
                e.ZIndex == currentLayerFromEditor);

            if (existingTile != null)
            {
                engine.CurrentScene.DestroyEntity(existingTile);
            }
            
            var tex = engine.AssetManager.GetTexture(selectedAsset);
            var entity = new Entity(snappedPos, tex, "NewTile");
            entity.Name = selectedAsset.Split('/').Last().Split('.').First();
            entity.ZIndex = currentLayerFromEditor;
            engine.CurrentScene.AddEntity(entity);
        }
    }
    
    public void DrawPreview(Engine engine)
    {
        if (!_hasPreview || string.IsNullOrEmpty(_lastAsset)) return;
        var tex = engine.AssetManager.GetTexture(_lastAsset);
        Raylib.DrawTextureV(tex, _lastSnappedPos, Raylib.Fade(Color.White, 0.5f));
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
