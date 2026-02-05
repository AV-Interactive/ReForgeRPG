using System.Numerics;
using ImGuiNET;
using Raylib_cs;
using ReForge.Engine.Core;
using ReForge.Engine.World;

namespace Reforge.Editor.Tools;

public class MapPainter
{
    public float GridSize = 32.0f;
    
    public void Update(Engine engine, string selectedAsset)
    {
        if(ImGui.IsMouseClicked(ImGuiMouseButton.Left) && !string.IsNullOrEmpty(selectedAsset))
        {
            Vector2 mousePos = ImGui.GetMousePos();
            Vector2 windowPos = ImGui.GetWindowPos();
            Vector2 contentOffset = ImGui.GetCursorStartPos();
            
            Vector2 relativeMousePos = mousePos - (windowPos + contentOffset);

            Vector2 snappedPos = new Vector2(
                MathF.Floor(relativeMousePos.X / GridSize) * GridSize,
                MathF.Floor(relativeMousePos.Y / GridSize) * GridSize
            );

            if (ImGui.IsWindowHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            {
                if (!string.IsNullOrEmpty(selectedAsset))
                {
                    var tex = engine.AssetManager.GetTexture(selectedAsset);
                    var entity = new Entity(snappedPos, tex, "NewTile");
                    engine.CurrentScene.AddEntity(entity);
                }
            }
        }
    }
    
    public void DrawPreview(Engine engine, string selectedAsset)
    {
        if (string.IsNullOrEmpty(selectedAsset)) return;

        Vector2 mousePos = ImGui.GetMousePos();
        Vector2 windowPos = ImGui.GetWindowPos();
        Vector2 contentOffset = ImGui.GetCursorStartPos();
        Vector2 relativeMousePos = mousePos - (windowPos + contentOffset);

        Vector2 snappedPos = new Vector2(
            MathF.Floor(relativeMousePos.X / GridSize) * GridSize,
            MathF.Floor(relativeMousePos.Y / GridSize) * GridSize
        );

        var tex = engine.AssetManager.GetTexture(selectedAsset);
        Raylib.DrawTextureV(tex, snappedPos, Raylib.Fade(Color.White, 0.5f));
    }
    
    public void DrawGrid()
    {
        Color gridColor = new Color(50, 50, 50, 255);

        for (int x = 0; x <= 1280; x += (int)GridSize)
        {
            Raylib.DrawLine(x, 0, x, 720, gridColor);
        }

        for (int y = 0; y <= 720; y += (int)GridSize)
        {
            Raylib.DrawLine(0, y, 1280, y, gridColor);
        }
    }
}
