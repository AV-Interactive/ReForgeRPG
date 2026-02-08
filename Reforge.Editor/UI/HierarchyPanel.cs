using System.Numerics;
using ImGuiNET;
using Raylib_cs;
using Reforge.Editor.Core;
using ReForge.Engine.World;

namespace Reforge.Editor.UI;

public class HierarchyPanel
{
    RenderTexture2D _viewportRes;
    public Vector2 WindowPosition { get; private set; }
    public Entity? SelectedEntity { get; set; }
    
    public void Draw(IEnumerable<Entity> entities, EditorContext ctx)
    {
        float heightPercentage = 0.30f;
        float minHeight = 200f;
        float hierarchyHeight = Math.Max(minHeight, Raylib.GetScreenHeight() * heightPercentage);
        ctx.HierarchyHeight = hierarchyHeight;
        ImGui.SetNextWindowPos(new Vector2(0, ctx.MenuBarHeight), ImGuiCond.Always);
        ImGui.SetNextWindowSize(new Vector2(ctx.SidebarWidth, hierarchyHeight), ImGuiCond.Always);

        if (ImGui.Begin("Hierarchy", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize))
        {
            WindowPosition = ImGui.GetWindowPos();
            foreach (Entity entity in entities)
            {
                if (ImGui.Selectable($"{entity.Name}##{entity.GetHashCode()}", entity == ctx.Hierarchy.SelectedEntity))
                {
                    ctx.Hierarchy.SelectedEntity = entity;
                }
            }
        }
        ImGui.End();
    }
}
