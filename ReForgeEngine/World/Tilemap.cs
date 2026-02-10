using System.Numerics;
using Raylib_cs;

namespace ReForge.Engine.World;

public class TileLayer
{
    public int[][] Data { get; set; }
    public bool IsCollidable { get; set;}
    public bool IsForeground { get; set;}
    public TileLayer() { }
    
    public TileLayer(int width, int height)
    {
        Data = new int[height][];
        for (int i = 0; i < height; i++)
        {
            Data[i] = new int[width];
        }
    }
    
    public void Resize(int newWidth, int newHeight)
    {
        int[][] newData = new int[newHeight][];
        for (int i = 0; i < newHeight; i++)
        {
            newData[i] = new int[newWidth];
            if (i < Data.Length)
            {
                Array.Copy(
                    Data[i], 
                    newData[i], 
                    Math.Min(Data[i].Length, newWidth)
                );
            }
        }
        Data = newData;
    }
}

public class Tilemap: Entity
{
    public List<TileLayer> Layers { get; set; } = new List<TileLayer>();
    public int TileSize { get; set; }
    
    [System.Text.Json.Serialization.JsonIgnore] 
    public new Texture2D Texture { get; set; }
    
    public Tilemap(): base() { }
    
    public Tilemap(Vector2 position, int tileSize, Texture2D texture, string texturePath) : base(position, texture, "Tilemap", texturePath)
    {
        TileSize = tileSize;
        this.Texture = texture;
    }
    
    public List<TileLayer> GetLayers() => Layers;
    public void AddLayer(TileLayer layer) => Layers.Add(layer);
    
    public override void Draw()
    {
        // Si aucune tuile à dessiner (toutes les layers vides), on dessine le sprite de base
        bool hasAnyTile = false;
        foreach (var layer in Layers)
        {
            int h = layer.Data.Length;
            if (h == 0) continue;
            int w = layer.Data[0].Length;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (layer.Data[y][x] != 0)
                    {
                        hasAnyTile = true;
                        break;
                    }
                }
                if (hasAnyTile) break;
            }
            if (hasAnyTile) break;
        }

        if (!hasAnyTile)
        {
            base.Draw();
            return;
        }

        foreach (TileLayer layer in Layers)
        {
            DrawLayer(layer);
        }
    }

    public void DrawLayer(TileLayer layer)
    {
        int gridHeight = layer.Data.Length;
        int gridWidth = layer.Data[0].Length;
        
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {   
                int tileId = layer.Data[y][x];

                if (tileId == 0) continue;

                Vector2 screenPos = new Vector2(
                    Position.X + (x * TileSize), 
                    Position.Y + (y * TileSize)
                );

                DrawTile(tileId, screenPos);
            }
        }
    }
    
    void DrawTile(int tileId, Vector2 screenPos)
    {
        if(Texture.Id == 0 || TileSize <= 0) return;
        
        int tilesPerRow = Texture.Width / TileSize;
        
        if(tilesPerRow <= 0) return;

        // Les IDs stockés dans la layer sont 1-based (0 = vide)
        int atlasIndex = tileId - 1;
        int column = atlasIndex % tilesPerRow;
        int row = atlasIndex / tilesPerRow;

        Rectangle sourceRec = new Rectangle(
            column * TileSize, 
            row * TileSize, 
            TileSize, 
            TileSize
        );

        Raylib.DrawTextureRec(Texture, sourceRec, screenPos, Color.White);
    }
}
