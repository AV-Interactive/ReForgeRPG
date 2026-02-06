using System.Reflection;
using System.Numerics;
using ImGuiNET;
using ReForge.Engine.World;
using ReForge.Engine.World.Behaviors;

namespace Reforge.Editor.UI;

public class InspectorPanel
{
    List<Type> _availableBehaviors = new List<Type>();
    
    public void Draw(Entity? selectedEntity)
    {
        ImGui.Begin("Inspecteur");
        if (selectedEntity == null)
        {
            ImGui.TextDisabled("Sélectionner une entité dans la hiérarchie.");
            ImGui.End();
            return;
        }
        
        ImGui.TextColored(new Vector4(0.4f, 0.8f, 1, 1), $"Editer: {selectedEntity}");
        ImGui.Separator();

        Vector2 pos = selectedEntity.Position;
        if (ImGui.DragFloat2("Position", ref pos)) selectedEntity.Position = pos;
        
        ImGui.Separator();
        ImGui.Text("Comportements");
        Behavior? behaviorToRemove = null;

        foreach (var behavior in selectedEntity.Behaviors)
        {
            ImGui.PushID(behavior.GetHashCode());
            
            bool open = ImGui.TreeNodeEx(behavior.GetType().Name, ImGuiTreeNodeFlags.DefaultOpen);
            
            ImGui.SameLine(ImGui.GetWindowWidth() - 30);
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.6f, 0.1f, 0.1f, 1.0f));
            if (ImGui.Button("X"))
            {
                behaviorToRemove = behavior;
            }
            ImGui.PopStyleColor();
            
            if (open)
            {
                DrawBehaviorEditor(behavior);
                ImGui.TreePop();
            }
            
            ImGui.PopID();
        }

        if (behaviorToRemove != null)
        {
            selectedEntity.RemoveBehavior(behaviorToRemove);
        }
        
        ImGui.Separator();
        if (ImGui.Button("Ajouter", new Vector2(-1, 25)))
        {
            RefreshBehaviorList();
            ImGui.OpenPopup("Ajouter un comportement");
        }

        if (ImGui.BeginPopupModal("Ajouter un comportement"))
        {
            ImGui.TextColored(new Vector4(0.4f, 0.8f, 1, 1), "Choisir un comportement");
            foreach (var type in _availableBehaviors)
            {
                if (!selectedEntity.Behaviors.Any(b => b.GetType() == type))
                {
                    if (ImGui.Selectable(type.Name))
                    {
                        var newBehavior = (Behavior)Activator.CreateInstance(type);
                        selectedEntity.AddBehavior(newBehavior);
                    }
                }
            }
            if (ImGui.Button("Annuler", new Vector2(-1, 0)))
            {
                ImGui.CloseCurrentPopup();
            }
            
            ImGui.EndPopup();
        }
        
        ImGui.End();
    }

    void DrawBehaviorEditor(Behavior behavior)
    {
        // PROPRIETES
        var properties = behavior.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach (var prop in properties)
        {
            if (!prop.CanWrite || !prop.CanRead) continue;

            var value = prop.GetValue(behavior);
        
            if (prop.PropertyType == typeof(float))
            {
                float f = (float)value!;
                if (ImGui.DragFloat(prop.Name, ref f)) prop.SetValue(behavior, f);
            }
            else if (prop.PropertyType == typeof(Vector2))
            {
                Vector2 v = (Vector2)value!;
                if (ImGui.DragFloat2(prop.Name, ref v)) prop.SetValue(behavior, v);
            }
            else if (prop.PropertyType == typeof(int))
            {
                int i = (int)value!;
                if (ImGui.DragInt(prop.Name, ref i)) prop.SetValue(behavior, i);
            }
            else if (prop.PropertyType == typeof(bool))
            {
                bool b = (bool)value!;
                if (ImGui.Checkbox(prop.Name, ref b)) prop.SetValue(behavior, b);
            }
            else if (prop.PropertyType == typeof(string))
            {
                string s = (string)value!;
                if (ImGui.InputText(prop.Name, ref s, 128)) prop.SetValue(behavior, s);
            }
            
        }
        
        // Variables
        var fields = behavior.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

        foreach (var field in fields)
        {
            var value = field.GetValue(behavior);

            if (field.FieldType == typeof(float))
            {
                float f = (float) value!;
                if (ImGui.DragFloat(field.Name, ref f)) field.SetValue(behavior, f);
            }
            else if (field.FieldType == typeof(int))
            {
                int i = (int)value!;
                if (ImGui.DragInt(field.Name, ref i)) field.SetValue(behavior, i);
            }
            else if (field.FieldType == typeof(bool))
            {
                bool b = (bool)value!;
                if (ImGui.Checkbox(field.Name, ref b)) field.SetValue(behavior, b);
            }
            else if (field.FieldType == typeof(string))
            {
                string s = (string)value!;
                if (ImGui.InputText(field.Name, ref s, 128)) field.SetValue(behavior, s);
            }
        }
    }

    void RefreshBehaviorList()
    {
        _availableBehaviors = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(Behavior).IsAssignableFrom(p) && !p.IsAbstract && p != typeof(Behavior))
            .ToList();
    }
}
