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
        if (selectedEntity == null) return;

        float alpha = (MathF.Sin(_blinkTimer * 10f) + 1.0f) / 2.0f;
        float width = EditorConfig.TileSize;
        float height = EditorConfig.TileSize;

        if (selectedEntity.Sprite != null && selectedEntity.Sprite.Texture.Id != 0)
        {
            width = selectedEntity.Sprite.Texture.Width;
            height = selectedEntity.Sprite.Texture.Height;
        }

        Rectangle highlight = new Rectangle(
            selectedEntity.Position.X,
            selectedEntity.Position.Y,
            width,
            height
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
            
            // Affichage du nom au-dessus de l'entité sélectionnée
            Raylib.DrawText(selectedEntity.Name, (int)selectedEntity.Position.X, (int)selectedEntity.Position.Y - 15, 10, Raylib.Fade(Color.White, alpha));
        }
    }
}
