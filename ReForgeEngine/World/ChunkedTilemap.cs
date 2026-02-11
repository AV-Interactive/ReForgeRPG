using System.Numerics;
using Raylib_cs;

namespace ReForge.Engine.World;

public class ChunkedTilemap : Entity
{
    public sealed class Chunk
    {
        public readonly int[][] Data; // [y][x]
        public Chunk(int size)
        {
            Data = new int[size][];
            for (int y = 0; y < size; y++)
            {
                Data[y] = new int[size];
            }
        }
    }

    public sealed class Layer
    {
        // Dictionnaire clairsemé de chunks pour cette layer
        public readonly Dictionary<(int cx, int cy), Chunk> Chunks = new();
    }

    public int TileSize { get; set; }
    public int ChunkSize { get; set; } = 64; // défaut raisonnable

    [System.Text.Json.Serialization.JsonIgnore]
    public new Texture2D Texture { get; set; }

    public List<Layer> Layers { get; } = new();

    public ChunkedTilemap() : base() { }

    public ChunkedTilemap(Vector2 position, int tileSize, Texture2D texture, string texturePath)
        : base(position, texture, "ChunkedTilemap", texturePath)
    {
        TileSize = tileSize;
        Texture = texture;
        // Par défaut, on crée une layer
        Layers.Add(new Layer());
    }

    public override Entity Clone()
    {
        var clone = new ChunkedTilemap(this.Position, this.TileSize, this.Texture, this.TexturePath)
        {
            Name = this.Name,
            ZIndex = this.ZIndex,
            ChunkSize = this.ChunkSize
        };

        foreach (var tag in this.Tags)
            clone.AddTag(tag);

        foreach (var behavior in this.Behaviors)
            clone.AddBehavior(behavior.Clone());

        // Copie superficielle des données (profonde par chunks)
        clone.Layers.Clear();
        foreach (var layer in Layers)
        {
            var newLayer = new Layer();
            foreach (var kvp in layer.Chunks)
            {
                var src = kvp.Value;
                var dst = new Chunk(ChunkSize);
                for (int y = 0; y < ChunkSize; y++)
                {
                    Array.Copy(src.Data[y], dst.Data[y], ChunkSize);
                }
                newLayer.Chunks[kvp.Key] = dst;
            }
            clone.Layers.Add(newLayer);
        }

        return clone;
    }

    // API principale
    public void EnsureLayer(int layerIndex)
    {
        while (Layers.Count <= layerIndex) Layers.Add(new Layer());
    }

    public void SetTile(int layerIndex, int gridX, int gridY, int tileId)
    {
        EnsureLayer(layerIndex);
        var (cx, cy, lx, ly) = ToChunkCoords(gridX, gridY);
        var chunk = GetOrCreateChunk(Layers[layerIndex], cx, cy);
        chunk.Data[ly][lx] = tileId; // 1-based attendu (0 = vide)
    }

    public int GetTile(int layerIndex, int gridX, int gridY)
    {
        if (layerIndex < 0 || layerIndex >= Layers.Count) return 0;
        var (cx, cy, lx, ly) = ToChunkCoords(gridX, gridY);
        var dict = Layers[layerIndex].Chunks;
        if (!dict.TryGetValue((cx, cy), out var chunk)) return 0;
        return chunk.Data[ly][lx];
    }

    public void ClearTile(int layerIndex, int gridX, int gridY)
    {
        SetTile(layerIndex, gridX, gridY, 0);
    }

    (int cx, int cy, int lx, int ly) ToChunkCoords(int gridX, int gridY)
    {
        int cs = ChunkSize;
        // gestion des négatifs: Division entière en C# tronque vers 0, on veut une division de plancher
        int cx = FloorDiv(gridX, cs);
        int cy = FloorDiv(gridY, cs);
        int lx = Mod(gridX, cs);
        int ly = Mod(gridY, cs);
        return (cx, cy, lx, ly);
    }

    static int FloorDiv(int a, int b)
    {
        int q = a / b;
        int r = a % b;
        if ((r != 0) && ((r > 0) != (b > 0))) q--;
        return q;
    }

    static int Mod(int a, int b)
    {
        int r = a % b;
        if (r < 0) r += Math.Abs(b);
        return r;
    }

    Chunk GetOrCreateChunk(Layer layer, int cx, int cy)
    {
        var key = (cx, cy);
        if (!layer.Chunks.TryGetValue(key, out var chunk))
        {
            chunk = new Chunk(ChunkSize);
            layer.Chunks[key] = chunk;
        }
        return chunk;
    }

    public override void Draw()
    {
        if (Texture.Id == 0 || TileSize <= 0) return;
        int tilesPerRow = Texture.Width / TileSize;
        if (tilesPerRow <= 0) return;

        // MVP: on parcourt tous les chunks existants (sparse). Culling caméra possible en amélioration.
        for (int li = 0; li < Layers.Count; li++)
        {
            foreach (var ((cx, cy), chunk) in Layers[li].Chunks)
            {
                DrawChunk(chunk, cx, cy, tilesPerRow);
            }
        }
    }

    void DrawChunk(Chunk chunk, int cx, int cy, int tilesPerRow)
    {
        int cs = ChunkSize;
        for (int ly = 0; ly < cs; ly++)
        {
            for (int lx = 0; lx < cs; lx++)
            {
                int tileId = chunk.Data[ly][lx];
                if (tileId == 0) continue;

                int atlasIndex = tileId - 1; // 1-based
                int column = atlasIndex % tilesPerRow;
                int row = atlasIndex / tilesPerRow;

                Rectangle sourceRec = new Rectangle(
                    column * TileSize,
                    row * TileSize,
                    TileSize,
                    TileSize
                );

                // Position monde = origine Tilemap + (coord grille globale * taille tuile)
                int gridX = cx * cs + lx;
                int gridY = cy * cs + ly;
                Vector2 screenPos = new Vector2(
                    Position.X + (gridX * TileSize),
                    Position.Y + (gridY * TileSize)
                );

                Raylib.DrawTextureRec(Texture, sourceRec, screenPos, Color.White);
            }
        }
    }
}
