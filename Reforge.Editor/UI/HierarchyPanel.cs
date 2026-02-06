using ImGuiNET;
using ReForge.Engine.World;

namespace Reforge.Editor.UI;

public class HierarchyPanel
{
    public Entity? SelectedEntity { get; set; }
    
    public void Draw(IEnumerable<Entity> entities)
    {
        ImGui.Begin("Hierarchy");
        foreach (Entity entity in entities)
        {
            if (ImGui.Selectable($"{entity.Name}##{entity.GetHashCode()}", entity == SelectedEntity))
            {
                SelectedEntity = entity;
            }
        }
        ImGui.End();
    }
}
