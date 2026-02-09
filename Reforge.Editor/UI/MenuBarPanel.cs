using System.Numerics;
using ImGuiNET;
using Reforge.Editor.Core;
using Reforge.Editor.Tools;
using ReForge.Engin.Core;
using ReForge.Engine.Core;

namespace Reforge.Editor.UI;

public class MenuBarPanel
{
    bool _showSavePopup = false;
    bool _showSaveScenePopup = false;
    string _projectNameBuffer = "";
    string _sceneNameBuffer = "";
    Engine _engine;
    
    public void Draw(Engine engine, EditorContext ctx)
    {
        _engine = engine;
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("Fichier"))
            {
                if (ImGui.MenuItem("Sauvegarder"))
                {
                    if (ProjectManager.IsSaved)
                    {
                        ProjectManager.SaveProject();
                    }
                    else
                    {
                        _showSavePopup = true;
                    }
                }
                
                if (ImGui.MenuItem("Sauvegarder la Scène actuelle"))
                {
                    if (ProjectManager.IsSaved)
                    {
                        if (ProjectManager.CurrentScene != null)
                        {
                            ProjectManager.SaveScene();
                        }
                        else
                        {
                            _showSaveScenePopup = true;
                        }
                    }
                    else
                    {
                        _showSavePopup = true;
                    }
                }
                
                ImGui.EndMenu();
            }
                
            ImGui.Separator();
                
                
            if (ctx.State == EditorApp.EditorState.Editing)
            {
                if (ImGui.MenuItem("Play"))
                {
                    ctx.SnapshotEntities.Clear();
                    foreach (var entity in engine.CurrentScene.Entities)
                    {
                        ctx.SnapshotEntities.Add(entity.Clone());
                    }
                    ctx.State = EditorApp.EditorState.Playing;
                }
            }
            else
            {
                if (ImGui.MenuItem("Stop"))
                {
                    ctx.Hierarchy.SelectedEntity = null;
                    engine.CurrentScene.Entities.Clear();
                    foreach (var entity in ctx.SnapshotEntities)
                    {
                        engine.CurrentScene.AddEntity(entity.Clone());
                    }
                    
                    Console.WriteLine($"Stop : Scène restaurée avec : {engine.CurrentScene.Entities.Count} entités.");
                    
                    ctx.State = EditorApp.EditorState.Editing;
                }
            }
                
            ImGui.Separator();
                
            if (EditorConfig.CurrentTool == EditorTool.Drawing)
            {
                if (ImGui.MenuItem("Selection")) EditorConfig.CurrentTool = EditorTool.Selection;
            }
            else
            {
                if (ImGui.MenuItem("Pinceau")) EditorConfig.CurrentTool = EditorTool.Drawing;
            }
            ImGui.EndMainMenuBar();
        }
        
        if (_showSavePopup) 
        {
            ImGui.OpenPopup("Enregistrer le projet");
        }
        if (_showSaveScenePopup) 
        {
            ImGui.OpenPopup("Enregistrer la Scene");
        }
        
        DrawSavePopup();
        DrawSaveScenePopup();
    }
    
    void DrawSavePopup()
    {
        if (ImGui.BeginPopupModal("Enregistrer le projet", ref _showSavePopup, ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.TextColored(new Vector4(1f, 0.8f, 0f, 1f), "Donner un nom au projet:");
            ImGui.InputText("##_projectNameBuffer", ref _projectNameBuffer, 50);
            if (ImGui.Button("Enregistrer"))
            {
                ProjectManager.CurrentProject.ProjectName = _projectNameBuffer;
                ProjectManager.SaveProject();
                _projectNameBuffer = "";
                _showSavePopup = false;
                ImGui.CloseCurrentPopup();
            }
            
            ImGui.SameLine();
            
            if(ImGui.Button("Annuler")) ImGui.CloseCurrentPopup();
            ImGui.EndPopup();
        }
    }
    
    void DrawSaveScenePopup()
    {
        if (ImGui.BeginPopupModal("Enregistrer la Scene", ref _showSaveScenePopup, ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.TextColored(new Vector4(1f, 0.8f, 0f, 1f), "Nommer la scène:");
            ImGui.InputText("##_sceneNameBuffer", ref _sceneNameBuffer, 50);
            if (ImGui.Button("Enregistrer"))
            {
                ProjectManager.CurrentSceneName = _sceneNameBuffer;
                ProjectManager.CurrentScene = _engine.CurrentScene;
                ProjectManager.SaveScene();
                _sceneNameBuffer = "";
                _showSaveScenePopup = false;
                ImGui.CloseCurrentPopup();
            }
            
            ImGui.SameLine();
            
            if(ImGui.Button("Annuler")) ImGui.CloseCurrentPopup();
            ImGui.EndPopup();
        }
    }
}
