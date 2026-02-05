using System.Numerics;
using Raylib_cs;

namespace ReForge.Engine.World;

public class TileLayer
{
    public int[,] Data;
    public bool IsCollidable;
    public bool IsForeground;
    
    public TileLayer(int width, int height)
    {
        Data = new int[width, height];
    }
}

public class Tilemap: Entity
{
    List<TileLayer> _layers = new List<TileLayer>();
    int _tileSize;
    Texture2D _tileset;
    
    public Tilemap(Vector2 position, int tileSize, Texture2D tileset) : base(position, tileset, "Tilemap")
    {
        _tileSize = tileSize;
        _tileset = tileset;
    }
    
    public void AddLayer(TileLayer layer) => _layers.Add(layer);
    
    public override void Draw()
    {
        foreach (TileLayer layer in _layers)
        {
            DrawLayer(layer);
        }
    }

    public void DrawLayer(TileLayer layer)
    {
        for (int y = 0; y < layer.Data.GetLength(1); y++)
        {
            for (int x = 0; x < layer.Data.GetLength(0); x++)
            {
                int tileId = layer.Data[x, y];

                if (tileId == 0) continue;

                Vector2 screenPos = new Vector2(
                    Position.X + (x * _tileSize), 
                    Position.Y + (y * _tileSize)
                );

                DrawTile(tileId, screenPos);
            }
        }
    }
    
    private void DrawTile(int tileId, Vector2 screenPos)
    {
        int tilesPerRow = _tileset.Width / _tileSize;

        int column = tileId % tilesPerRow;
        int row = tileId / tilesPerRow;

        // 3. Créer le rectangle source (quelle zone de l'image on prend)
        Rectangle sourceRec = new Rectangle(
            column * _tileSize, 
            row * _tileSize, 
            _tileSize, 
            _tileSize
        );

        Raylib.DrawTextureRec(_tileset, sourceRec, screenPos, Color.White);
    }
}
