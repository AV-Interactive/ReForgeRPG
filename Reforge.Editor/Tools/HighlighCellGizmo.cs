using System.Numerics;
using Raylib_cs;
using Reforge.Editor.Core;
using ReForge.Engine.World;

namespace Reforge.Editor.Tools;

public class HighlighCellGizmo
{
    float _blinkTimer = 0;

    public void UpdateTimer()
    {
        _blinkTimer += Raylib.GetFrameTime();
    }
    
    public void Draw(Entity? selectedEntity, bool forceHover = false)
    {
        float alpha = (MathF.Sin(_blinkTimer* 10f) + 1.0f) / 2.0f;

        Rectangle highlight = new Rectangle(
            selectedEntity.Position.X,
            selectedEntity.Position.Y,
            EditorConfig.GridSize,
            EditorConfig.GridSize
        );
        
        if (forceHover)
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
