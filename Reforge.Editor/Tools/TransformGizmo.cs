using System.Numerics;
using Raylib_cs;
using Reforge.Editor.Core;
using ReForge.Engine.World;

namespace Reforge.Editor.Tools;

public class TransformGizmo
{
    float _blinkTimer = 0;

    public void Update(Entity? selectedEntity, Vector2 viewportPos)
    {
        if (selectedEntity == null) return;

        Vector2 mousePos = Raylib.GetMousePosition();
        Vector2 relativeMousePos = mousePos - viewportPos;

        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            Vector2 snappedPos = new Vector2(
                MathF.Floor(relativeMousePos.X / EditorConfig.GridSize) * EditorConfig.GridSize,
                MathF.Floor(relativeMousePos.Y / EditorConfig.GridSize) * EditorConfig.GridSize
            );
            
            selectedEntity.Position = snappedPos;
        }
    }

    public void Draw(Entity? selectedEntity)
    {
        if (selectedEntity == null) return;
        
        _blinkTimer += Raylib.GetFrameTime();
        float alpha = (MathF.Sin(_blinkTimer* 10f) + 1.0f) / 2.0f;

        Rectangle highlight = new Rectangle(
            selectedEntity.Position.X,
            selectedEntity.Position.Y,
            EditorConfig.GridSize,
            EditorConfig.GridSize
        );
        
        Raylib.DrawRectangleLinesEx(highlight, 2, Raylib.Fade(Color.White, alpha));
        Raylib.DrawRectangleRec(highlight, Raylib.Fade(Color.SkyBlue, 0.2f * alpha));
    }
}
