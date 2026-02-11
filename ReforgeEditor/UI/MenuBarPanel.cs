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
    bool _showProjectSettingsPopup = false;
    string _projectNameBuffer = "";
    string _sceneNameBuffer = "";
    int _tileSizeBuffer = 32;
    Engine _engine;
    
    public void Draw(Engine engine, EditorContext ctx)
    {
        _engine = engine;
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("Fichier"))
            {
                if (ctx.State == EditorApp.EditorState.Editing)
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
                }
                else
                {
                    ImGui.TextDisabled("Sauvegarder");
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

            if (ImGui.BeginMenu("Projet"))
            {
                if (ImGui.MenuItem("Paramètres du projet"))
                {
                    if (ProjectManager.CurrentProject != null)
                    {
                        _tileSizeBuffer = ProjectManager.CurrentProject.TileSize;
                        _showProjectSettingsPopup = true;
                    }
                }
                ImGui.EndMenu();
            }
                
            ImGui.Separator();
            
            // Zoom controls
            ImGui.Text("Zoom:");
            ImGui.SetNextItemWidth(40);
            if (ImGui.Button("-"))
            {
                ctx.Zoom -= 0.1f;
                if (ctx.Zoom < 0.1f) ctx.Zoom = 0.1f;
            }
            ImGui.SameLine();
            ImGui.Text($"{ctx.Zoom:P0}");
            ImGui.SameLine();
            if (ImGui.Button("+"))
            {
                ctx.Zoom += 0.1f;
                if (ctx.Zoom > 10.0f) ctx.Zoom = 10.0f;
            }
            ImGui.SameLine();
            if (ImGui.Button("100%"))
            {
                ctx.Zoom = 1.0f;
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
                    ctx.SelectedEntities.Clear()
                        ;
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
                if (ImGui.MenuItem("Sélection")) EditorConfig.CurrentTool = EditorTool.Selection;
                
                ImGui.Separator();
                
                if (ImGui.RadioButton("Pinceau", EditorConfig.CurrentPaintingMode == PaintingMode.Brush))
                    EditorConfig.CurrentPaintingMode = PaintingMode.Brush;
                
                ImGui.SameLine();
                
                if (ImGui.RadioButton("Rectangle", EditorConfig.CurrentPaintingMode == PaintingMode.Rectangle))
                    EditorConfig.CurrentPaintingMode = PaintingMode.Rectangle;
                
                ImGui.Separator();

                if (ImGui.MenuItem("Effacer"))
                {
                    ctx.SelectedEntities.Clear();
                    EditorConfig.CurrentPaintingMode = PaintingMode.Eraser;
                }
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
        
        if (_showProjectSettingsPopup) 
        {
            ImGui.OpenPopup("Paramètres du Projet");
        }
        
        DrawSavePopup();
        DrawSaveScenePopup();
        DrawProjectSettingsPopup();
    }
    
    void DrawProjectSettingsPopup()
    {
        if (ImGui.BeginPopupModal("Paramètres du Projet", ref _showProjectSettingsPopup, ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.TextColored(new Vector4(1f, 0.8f, 0f, 1f), "Taille de la grille (px):");
            if (ImGui.InputInt("##_tileSizeBuffer", ref _tileSizeBuffer))
            {
                if (_tileSizeBuffer < 1) _tileSizeBuffer = 1;
                if (_tileSizeBuffer > 256) _tileSizeBuffer = 256;
            }

            ImGui.Separator();

            if (ImGui.Button("Appliquer"))
            {
                if (ProjectManager.CurrentProject != null)
                {
                    ProjectManager.CurrentProject.TileSize = _tileSizeBuffer;
                    EditorConfig.TileSize = _tileSizeBuffer;
                }
                _showProjectSettingsPopup = false;
                ImGui.CloseCurrentPopup();
            }
            
            ImGui.SameLine();
            
            if(ImGui.Button("Annuler")) 
            {
                _showProjectSettingsPopup = false;
                ImGui.CloseCurrentPopup();
            }
            ImGui.EndPopup();
        }
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
