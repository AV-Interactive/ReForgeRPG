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
            int i = 0;
            foreach (Entity entity in entities)
            {
                bool isSelected = ctx.SelectedEntities.Contains(entity);
                if (ImGui.Selectable($"{entity.Name}##{i}_{entity.GetHashCode()}", isSelected))
                {
                    if (ImGui.GetIO().KeyCtrl)
                    {

                        if (isSelected) ctx.SelectedEntities.Remove(entity);
                        else ctx.SelectedEntities.Add(entity);
                    }
                    else
                    {
                        ctx.SelectedEntities.Clear();
                        ctx.SelectedEntities.Add(entity);
                    }
                }
                i++;
            }
        }
        ImGui.End();
    }
}
