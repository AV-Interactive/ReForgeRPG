namespace Reforge.Editor.Core;

public enum EditorTool
{
    Drawing,
    Selection
}

public static class EditorConfig
{
    public static float GridSize = 32.0f;
    public static EditorTool CurrentTool = EditorTool.Selection;
}
