using System.Numerics;
using Raylib_cs;
using Reforge.Editor.Core;
using ReForge.Engine.World;
using System.Linq;

namespace Reforge.Editor.Tools;

public class EditorSelector
{
    public int SelectedTile { get; set; } = 0;
    public Entity? GetEntityAt(Scene scene, Vector2 worldMousePos, int layerIndex)
    {
        // Priorité: essayer de picker une Tilemap sur la couche demandée
        var tilemap = scene.Entities
            .OfType<Tilemap>()
            .FirstOrDefault(t => t.ZIndex == layerIndex);

        if (tilemap != null && tilemap.Layers.Count > 0)
        {
            int gx = (int)MathF.Floor((worldMousePos.X - tilemap.Position.X) / tilemap.TileSize);
            int gy = (int)MathF.Floor((worldMousePos.Y - tilemap.Position.Y) / tilemap.TileSize);

            var layer = tilemap.Layers[0];
            if (gx >= 0 && gy >= 0 && gy < layer.Data.Length && gx < layer.Data[0].Length)
            {
                if (layer.Data[gy][gx] != 0)
                {
                    return tilemap; // Retourne la Tilemap si une tuile non-vide est sous la souris
                }
            }
        }

        // Fallback: ancienne logique pour les entités unitaires alignées sur la grille
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
