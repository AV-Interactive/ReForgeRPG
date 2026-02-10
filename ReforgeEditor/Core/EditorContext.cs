using Reforge.Editor.Tools;
using Reforge.Editor.UI;
using ReForge.Engine.World;

namespace Reforge.Editor.Core;

public class EditorContext
{
    public EditorApp.EditorState State { get; set; }
    public int CurrentLayer { get; set; }
    public MapPainter MapPainter { get; set; }
    public HierarchyPanel Hierarchy { get; set; }
    public HighlighCellGizmo Gizmo { get; set; }
    public float SidebarWidth { get; set; }
    public float HierarchyHeight { get; set; }
    public float BrowserContentHeight { get; set; }
    public float InspectorWidth { get; set; }
    public float MenuBarHeight { get; set; } = 20;
    public ContentBrowser ContentBrowser { get; set; }
    public List<Entity> SnapshotEntities { get; set; } = new List<Entity>();
    public List<Entity> SelectedEntities { get; set; } = new List<Entity>();
    public EditorSelector EditorSelector { get; set; }
    public ReForge.Engine.World.Scene CurrentScene { get; set; }
    public int SelectedTile { get; set; }
}
