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
        
        // Edition spécifique Tilemap: taille de map (W,H) par layer et taille de tuile
        if (selectedEntity is Tilemap tilemapEntity)
        {
            ImGui.TextColored(new Vector4(0.8f, 0.8f, 1f, 1f), "Tilemap");

            int ts = tilemapEntity.TileSize;
            if (ImGui.InputInt("Taille de tuile", ref ts))
            {
                if (ts > 0) tilemapEntity.TileSize = ts;
            }

            for (int li = 0; li < tilemapEntity.Layers.Count; li++)
            {
                var layer = tilemapEntity.Layers[li];
                int currentH = layer.Data?.Length ?? 0;
                int currentW = currentH > 0 ? layer.Data[0].Length : 0;

                ImGui.PushID(li);
                ImGui.Separator();
                ImGui.Text($"Layer {li}");

                int newW = currentW;
                int newH = currentH;
                ImGui.InputInt("Largeur (tuiles)", ref newW);
                ImGui.InputInt("Hauteur (tuiles)", ref newH);

                if (ImGui.Button("Redimensionner"))
                {
                    newW = System.Math.Max(1, newW);
                    newH = System.Math.Max(1, newH);
                    layer.Resize(newW, newH);
                }
                ImGui.PopID();
            }

            ImGui.Separator();
        }

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
            
            // Isolation d'ID par propriété pour éviter les replis involontaires (ex: BoxCollider)
            ImGui.PushID(prop.Name);
        
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
                ImGui.PushID($"{prop.Name}");
                if(ImGui.TreeNode($"Commandes de {prop.Name} ({list.Count})##{prop.Name}"))
                {
                    if (ImGui.Button($"+ Ajouter##{prop.Name}"))
                    {
                        list.Add(new ActionCommand());
                    }

                    for (int i = 0; i < list.Count; i++)
                    {
                        var stableId = list[i].Id;
                        ImGui.PushID(stableId);
                        if (ImGui.TreeNode($"Commande {i} : {list[i].Verb}##TreeNode_{stableId}"))
                        {
                            DrawActionCommandEditor(list[i], list, i);
                            if (ImGui.Button($"Supprimer##Delete_{stableId}"))
                            {
                                list.RemoveAt(i);
                                ImGui.TreePop();
                                ImGui.PopID();
                                break;
                            }
                            ImGui.TreePop();
                        }
                        ImGui.PopID();
                    }
                    ImGui.TreePop();
                }
                ImGui.PopID();
            }
            else if (prop.PropertyType == typeof(List<ActionCondition>))
            {
                var list = (List<ActionCondition>)value!;
                ImGui.PushID($"{prop.Name}");
                if (ImGui.TreeNode($"Conditions de {prop.Name} ({list.Count})##{prop.Name}"))
                {
                    if (ImGui.Button($"+ Ajouter Condition##{prop.Name}"))
                    {
                        list.Add(new ActionCondition());
                    }

                    for (int i = 0; i < list.Count; i++)
                    {
                        var stableId = list[i].Id;
                        ImGui.PushID(stableId);
                        if (ImGui.TreeNode($"Condition {i} : {list[i].Key}##TreeNode_{stableId}"))
                        {
                            DrawActionConditionEditor(list[i], list, i);
                            ImGui.TreePop();
                        }
                        ImGui.PopID();
                    }
                    ImGui.TreePop();
                }
                ImGui.PopID();
            }
            
            ImGui.PopID();
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
        var stableId = actionCommand.Id;
        ImGui.PushID(stableId);
        string[] verbNames = Enum.GetNames(typeof(ActionVerb));
        int currentVerbIndex = (int)actionCommand.Verb;
        if (ImGui.Combo($"Verbe##ComboVerb_{stableId}", ref currentVerbIndex, verbNames, verbNames.Length))
        {
            actionCommand.Verb = (ActionVerb)currentVerbIndex;
        }
        
        bool targetSelf = actionCommand.TargetSelf;
        if (ImGui.Checkbox($"Cibler soi-même ##{stableId}", ref targetSelf))
        {
            actionCommand.TargetSelf = targetSelf;
        }

        if (!actionCommand.TargetSelf)
        {
            ImGui.TextColored(new Vector4(1f, 0.8f, 0f, 1f), "Ciblage Global par Tag");
            
            string tag = actionCommand.TargetTag;
            if (ImGui.InputText($"Tag cible ##{stableId}", ref tag, 64))
            {
                actionCommand.TargetTag = tag;
            }
        }

        switch (actionCommand.Verb)
        {
            case ActionVerb.Teleport:
                Vector2 destination = actionCommand.Destination;
                if(ImGui.DragFloat2($"Destination##{stableId}", ref destination))
                {
                    actionCommand.Destination = destination;
                }
                break;
            case ActionVerb.SetSwitch:
            case ActionVerb.SetVariable:
            case ActionVerb.AddValueVariable:
            case ActionVerb.SubtractValueVariable:
                string key = actionCommand.Key;
                if (ImGui.InputText($"Clé (Variable/Switch)##{stableId}", ref key, 64))
                {
                    actionCommand.Key = key;
                }
                float val = actionCommand.Value;
                if (ImGui.DragFloat($"Valeur##{stableId}", ref val))
                {
                    actionCommand.Value = val;
                }
                break;
            case ActionVerb.Destroy:
                break;
            case ActionVerb.ToggleActive:
                break;
        }
        ImGui.PopID();
    }

    void DrawActionConditionEditor(ActionCondition condition, List<ActionCondition> list, int i)
    {
        var stableId = condition.Id;
        ImGui.PushID($"Condition_{stableId}");
        
        string[] typeNames = Enum.GetNames(typeof(ActionConditionType));
        int typeIndex = (int)condition.Type;
        if (ImGui.Combo($"Type##ComboType_{stableId}", ref typeIndex, typeNames, typeNames.Length))
        {
            condition.Type = (ActionConditionType)typeIndex;
        }

        string key = condition.Key;
        if (ImGui.InputText($"Clé##Input_{stableId}", ref key, 64))
        {
            condition.Key = key;
        }

        if (condition.Type == ActionConditionType.Variable)
        {
            string[] opNames = Enum.GetNames(typeof(ConditionOperator));
            int opIndex = (int)condition.Operator;
            if (ImGui.Combo($"Opérateur##ComboOp_{stableId}", ref opIndex, opNames, opNames.Length))
            {
                condition.Operator = (ConditionOperator)opIndex;
            }
        }

        float val = condition.Value;
        if (condition.Type == ActionConditionType.Switch)
        {
            bool bVal = val != 0;
            if (ImGui.Checkbox($"Valeur Cible##Check_{stableId}", ref bVal))
            {
                condition.Value = bVal ? 1 : 0;
            }
        }
        else
        {
            if (ImGui.DragFloat($"Valeur Cible##Drag_{stableId}", ref val))
            {
                condition.Value = val;
            }
        }

        if (ImGui.Button($"Supprimer Condition##Delete_{stableId}"))
        {
            list.RemoveAt(i);
        }

        ImGui.PopID();
        ImGui.Separator();
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
