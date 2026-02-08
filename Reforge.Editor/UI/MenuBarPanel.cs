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
    string _projectNameBuffer = "";
    public void Draw(Engine _engine, EditorContext ctx)
    {
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
                
                if (ImGui.MenuItem("Sauvegarder la Scène"))
                {
                    string fullPath = Path.Combine(ProjectPaths.Scenes, "01.scn");
                    SceneSerializer.Save(_engine.CurrentScene, fullPath);
                }
                //if (ImGui.MenuItem("Quitter")) _running = false;
                
                ImGui.EndMenu();
            }
                
            ImGui.Separator();
                
                
            if (ctx.State == EditorApp.EditorState.Editing)
            {
                if (ImGui.MenuItem("Play")) ctx.State = EditorApp.EditorState.Playing;
            }
            else
            {
                if (ImGui.MenuItem("Stop")) ctx.State = EditorApp.EditorState.Editing;
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
        DrawSavePopup();
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
}
