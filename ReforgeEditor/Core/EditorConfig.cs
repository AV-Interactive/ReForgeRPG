namespace Reforge.Editor.Core;

public enum EditorTool
{
    Drawing,
    Selection
}

public enum PaintingMode
{
    Brush,
    Rectangle,
    Eraser
}

public static class EditorConfig
{
    public static float GridSize = 32.0f;
    public static EditorTool CurrentTool = EditorTool.Selection;
    public static PaintingMode CurrentPaintingMode = PaintingMode.Brush;
}
