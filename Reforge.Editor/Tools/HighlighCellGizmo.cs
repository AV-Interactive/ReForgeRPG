using System.Numerics;
using Raylib_cs;
using Reforge.Editor.Core;
using ReForge.Engine.World;

namespace Reforge.Editor.Tools;

public class HighlighCellGizmo
{
    float _blinkTimer = 0;
    bool _isHovered = false;

    public void Update(Entity? selectedEntity, Vector2 viewportPos)
    {
        if (selectedEntity == null)
        {
            _isHovered = false;
            return;
        }
        
        Vector2 relativeMousePos = ImGuiNET.ImGui.GetMousePos() - viewportPos;
        
        _isHovered = relativeMousePos.X >= selectedEntity.Position.X &&
                     relativeMousePos.X <  selectedEntity.Position.X + EditorConfig.GridSize &&
                     relativeMousePos.Y >= selectedEntity.Position.Y &&
                     relativeMousePos.Y <  selectedEntity.Position.Y + EditorConfig.GridSize;
    }

    public void Draw(Entity? selectedEntity)
    {
        _blinkTimer += Raylib.GetFrameTime();
        float alpha = (MathF.Sin(_blinkTimer* 10f) + 1.0f) / 2.0f;

        Rectangle highlight = new Rectangle(
            selectedEntity.Position.X,
            selectedEntity.Position.Y,
            EditorConfig.GridSize,
            EditorConfig.GridSize
        );
        
        if (_isHovered)
        {
            // alpha 0.5
            Raylib.DrawRectangleLinesEx(highlight, 2, Raylib.Fade(Color.White, .5f));
            Raylib.DrawRectangleRec(highlight, Raylib.Fade(Color.Gold, 0.2f * .5f));
        }
        else
        {
            Raylib.DrawRectangleLinesEx(highlight, 2, Raylib.Fade(Color.White, alpha));
            Raylib.DrawRectangleRec(highlight, Raylib.Fade(Color.SkyBlue, 0.2f * alpha));
        }
    }
}
