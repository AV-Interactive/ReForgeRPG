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
        float tileSize = EditorConfig.TileSize;

        // 1) Priorité aux entités "unitaires" (non Tilemap)
        // On cherche d'abord sur la couche active, puis sur les autres si rien n'est trouvé
        var entitiesToSearch = scene.Entities
            .Where(e => e is not Tilemap)
            .OrderByDescending(e => e.ZIndex == layerIndex) // Couche active en premier
            .ThenByDescending(e => e.Id); // Plus récente en cas de superposition

        var topEntity = entitiesToSearch.FirstOrDefault(e => {
                float width = tileSize;
                float height = tileSize;

                if (e.Sprite != null && e.Sprite.Texture.Id != 0)
                {
                    width = e.Sprite.Texture.Width;
                    height = e.Sprite.Texture.Height;
                }

                // Tolérance de 2 pixels pour faciliter la sélection
                float margin = 2.0f;
                return worldMousePos.X >= e.Position.X - margin && worldMousePos.X < e.Position.X + width + margin &&
                       worldMousePos.Y >= e.Position.Y - margin && worldMousePos.Y < e.Position.Y + height + margin;
            });

        if (topEntity != null)
            return topEntity;

        // 2) Sinon, si une Tilemap possède une tuile non vide sous la souris, on retourne la Tilemap
        var tilemaps = scene.Entities
            .OfType<Tilemap>()
            .OrderByDescending(t => t.ZIndex == layerIndex); // Priorité à la couche active

        foreach (var tilemap in tilemaps)
        {
            if (tilemap.Layers.Count > 0)
            {
                int gx = (int)MathF.Floor((worldMousePos.X - tilemap.Position.X) / tilemap.TileSize);
                int gy = (int)MathF.Floor((worldMousePos.Y - tilemap.Position.Y) / tilemap.TileSize);

                foreach (var layer in tilemap.Layers)
                {
                    if (gy >= 0 && gy < layer.Data.Length && gx >= 0 && gx < layer.Data[gy].Length)
                    {
                        if (layer.Data[gy][gx] != 0)
                        {
                            return tilemap; 
                        }
                    }
                }
            }
        }

        return null;
    }

    public Entity? UpdateSelection(Scene scene, Vector2 viewportPos, int layerIndex)
    {
        if(!Raylib.IsMouseButtonDown(MouseButton.Left)) return null;
        
        Vector2 mousePos = Raylib.GetMousePosition();
        Vector2 relativeMousePos = mousePos - viewportPos;
        
        return GetEntityAt(scene, relativeMousePos, layerIndex);
    }
}
