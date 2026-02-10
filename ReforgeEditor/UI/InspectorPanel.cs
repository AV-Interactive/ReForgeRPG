using System.Reflection;
using System.Numerics;
using ImGuiNET;
using Raylib_cs;
using Reforge.Editor.Core;
using ReForge.Engine.Core;
using ReForge.Engine.World;
using ReForge.Engine.World.Behaviors;

namespace Reforge.Editor.UI;

public class InspectorPanel
{
    List<Type> _availableBehaviors = new List<Type>();
    string newTag = "";

    public void Draw(Entity? selectedEntity, EditorContext ctx)
    {
        float windowWidth = Raylib.GetScreenWidth();
        float posX = windowWidth - ctx.InspectorWidth;

        ImGui.SetNextWindowPos(new Vector2(posX, ctx.MenuBarHeight), ImGuiCond.Always);
        ImGui.SetNextWindowSize(new Vector2(ctx.InspectorWidth, Raylib.GetScreenHeight()), ImGuiCond.Always);

        ImGui.Begin("Inspecteur");

        if (ctx.SelectedEntities.Count == 0)
        {
            ImGui.TextDisabled("Sélectionner une ou plusieurs entités.");
            ImGui.End();
            return;
        }

        if (ctx.SelectedEntities.Count > 1)
        {
            DrawMultiSelectionHeader(ctx);
        }
        else
        {
            DrawSingleEntityEditor(ctx);
        }
        ImGui.End();
    }

    void DrawMultiSelectionHeader(EditorContext ctx)
    {
        ImGui.TextColored(new Vector4(0.4f, 0.8f, 1, 1), $"{ctx.SelectedEntities.Count} Objets sélectionnés.");
        ImGui.Separator();

        ImGui.Text("Actions groupées :");

        if (ImGui.Button("Ajouter un comportement", new Vector2(-1, 25)))
        {
            RefreshBehaviorList();
            ImGui.OpenPopup("Ajouter un comportement");
        }
        DrawBehaviorSelector(ctx.SelectedEntities);
    }
    
    void DrawSingleEntityEditor(EditorContext ctx)
    {
        var selectedEntity = ctx.SelectedEntities[0];
        
        string name = selectedEntity.Name;
        if (ImGui.InputText("Nom", ref name, 64))
        {
            selectedEntity.Name = name;
        }

        Vector2 pos = selectedEntity.Position;
        if (ImGui.DragFloat2("Position", ref pos)) selectedEntity.Position = pos;
        
        ImGui.Text($"Tags:");
        int tagToRemove = -1;
        for (int i = 0; i < selectedEntity.Tags.Count; i++)
        {
            ImGui.SameLine();
            if (ImGui.Button($"{selectedEntity.Tags[i]}##{i}"))
            {
                tagToRemove = i;
            }
        }

        if (tagToRemove != -1)
        {
            selectedEntity.Tags.RemoveAt(tagToRemove);
        }
        
        ImGui.InputText("##NewTag", ref newTag, 32);
        ImGui.SameLine();
        if (ImGui.Button("+"))
        {
            selectedEntity.AddTag(newTag);
            newTag = "";
        }

        ImGui.Separator();
        ImGui.Text("Comportements");
        Behavior? behaviorToRemove = null;

        foreach (var behavior in selectedEntity.Behaviors)
        {
            if (behavior.GetType().GetCustomAttribute<HiddenBehaviorAttribute>() != null) continue;
            
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
                DrawBehaviorEditor(behavior, ctx);
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
        
        DrawBehaviorSelector(ctx.SelectedEntities);
    }

    void DrawBehaviorEditor(Behavior behavior, EditorContext ctx)
    {
        // Bouton spécial pour WorldBounds
        if (behavior is WorldBounds wb)
        {
            if (ImGui.Button("Ajuster à la Tilemap"))
            {
                var tilemap = ctx.CurrentScene.Entities.OfType<Tilemap>().FirstOrDefault();
                if (tilemap != null && tilemap.Layers.Count > 0)
                {
                    var layer = tilemap.Layers[0];
                    float mapWidth = layer.Data[0].Length * tilemap.TileSize;
                    float mapHeight = layer.Data.Length * tilemap.TileSize;

                    wb.MinBounds = tilemap.Position;
                    wb.MaxBounds = tilemap.Position + new Vector2(mapWidth, mapHeight);
                }
            }
            ImGui.Separator();
        }

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
            else if (prop.PropertyType == typeof(List<ActionCommand>))
            {
                var list = (List<ActionCommand>)value!;
                if(ImGui.TreeNode($"Commandes de {prop.Name} ({list.Count})"))
                {
                    if (ImGui.Button($"+ Ajouter"))
                    {
                        list.Add(new ActionCommand());
                    }

                    for (int i = 0; i < list.Count; i++)
                    {
                        DrawActionCommandEditor(list[i], list, i);
                    }
                    ImGui.TreePop();
                }
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

    void DrawActionCommandEditor(ActionCommand actionCommand, List<ActionCommand> list, int i)
    {
        string[] verbNames = Enum.GetNames(typeof(ActionVerb));
        int currentVerbIndex = (int)actionCommand.Verb;

        if (ImGui.Combo("Verbe", ref currentVerbIndex, verbNames, verbNames.Length))
        {
            actionCommand.Verb = (ActionVerb)currentVerbIndex;
        }
        
        bool targetSelf = actionCommand.TargetSelf;
        if (ImGui.Checkbox($"Cibler soi-même ##{i}", ref targetSelf))
        {
            actionCommand.TargetSelf = targetSelf;
        }

        if (!actionCommand.TargetSelf)
        {
            ImGui.TextColored(new Vector4(1f, 0.8f, 0f, 1f), "Ciblage Global par Tag");
            
            string tag = actionCommand.TargetTag;
            if (ImGui.InputText($"Tag cible ##{i}", ref tag, 64))
            {
                actionCommand.TargetTag = tag;
            }
        }

        switch (actionCommand.Verb)
        {
            case ActionVerb.Teleport:
                Vector2 destination = actionCommand.Destination;
                if(ImGui.DragFloat2($"Destination##{i}", ref destination))
                {
                    actionCommand.Destination = destination;
                }
                break;
            case ActionVerb.Destroy:
                break;
            case ActionVerb.ToggleActive:
                break;
        }
    }

    void RefreshBehaviorList()
    {
        _availableBehaviors = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(Behavior).IsAssignableFrom(p) 
                        && !p.IsAbstract 
                        && p != typeof(Behavior)
                        && p.GetCustomAttribute<HiddenBehaviorAttribute>() == null
                        && p.GetCustomAttribute<HiddenSelectableBehaviorAttribute>() == null)
            .ToList();
    }
    
    void DrawBehaviorSelector(List<Entity> targets)
    {
        if (ImGui.BeginPopupModal("Ajouter un comportement"))
        {
            ImGui.TextColored(new Vector4(0.4f, 0.8f, 1, 1), "Choisir un comportement");
            foreach (var type in _availableBehaviors)
            {
                if (ImGui.Selectable(type.Name))
                {
                    foreach (var entity in targets)
                    {
                        if (!entity.Behaviors.Any(b => b.GetType() == type))
                        {
                            entity.AddBehavior((Behavior)Activator.CreateInstance(type));
                        }
                    }
                    ImGui.CloseCurrentPopup();
                }
            }
            if (ImGui.Button("Annuler", new Vector2(-1, 0)))
            {
                ImGui.CloseCurrentPopup();
            }
            
            ImGui.EndPopup();
        }
    }
}
