using System.Numerics;
using Raylib_cs;

namespace Reforge.Editor.Core;

public static class EditorMath
{
    public static Vector2 SnapToGrid(Vector2 position)
    {
        return new Vector2(
            MathF.Floor(position.X / EditorConfig.TileSize) * EditorConfig.TileSize,
            MathF.Floor(position.Y / EditorConfig.TileSize) * EditorConfig.TileSize
        );
    }

    public static Vector2 SnapToGridRelativePos(Vector2 relativePosition)
    {
        return SnapToGrid(relativePosition);
    }
}
