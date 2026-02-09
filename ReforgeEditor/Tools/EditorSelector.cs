using System.Numerics;
using Raylib_cs;
using Reforge.Editor.Core;
using ReForge.Engine.World;

namespace Reforge.Editor.Tools;

public class EditorSelector
{
    public Entity? GetEntityAt(Scene scene, Vector2 worldMousePos, int layerIndex)
    {
        return scene.Entities
            .Where(e => e.Position == worldMousePos)
            .OrderByDescending(e => e.ZIndex)
            .FirstOrDefault();
    }

    public Entity? UpdateSelection(Scene scene, Vector2 viewportPos, int layerIndex)
    {
        if(!Raylib.IsMouseButtonDown(MouseButton.Left)) return null;
        
        Vector2 mousePos = Raylib.GetMousePosition();
        Vector2 relativeMousePos = mousePos - viewportPos;
        
        Vector2 snappedPos = EditorMath.SnapToGrid(relativeMousePos);
        
        return GetEntityAt(scene, snappedPos, layerIndex);
    }
}
