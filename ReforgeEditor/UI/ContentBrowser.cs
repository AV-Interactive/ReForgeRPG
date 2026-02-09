using System.Numerics;
using ImGuiNET;
using Raylib_cs;
using Reforge.Editor.Core;
using ReForge.Engin.Core;
using ReForge.Engine.Core;
using rlImGui_cs;

namespace Reforge.Editor.UI;

public class ContentBrowser
{
    public string SelectedAsset { get; set; } = "";
    AssetType _currentType = AssetType.Actors;

    public void Draw(Engine engine, EditorContext ctx)
    {
        ctx.BrowserContentHeight = Raylib.GetScreenHeight() - ctx.HierarchyHeight - ctx.MenuBarHeight - 120;
        ImGui.SetNextWindowPos(new Vector2(0, ctx.MenuBarHeight + ctx.HierarchyHeight), ImGuiCond.Always);
        ImGui.SetNextWindowSize(new Vector2(ctx.SidebarWidth, ctx.BrowserContentHeight), ImGuiCond.Always);

        if (ImGui.Begin("Explorateur", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize))
        {
            if (ImGui.BeginTabBar("Ressources"))
            {
                foreach (AssetType type in Enum.GetValues(typeof(AssetType)))
                {
                    if (ImGui.BeginTabItem(type.ToString()))
                    {
                        _currentType = type;
                        ImGui.EndTabItem();
                    }
                }
                
                string finalPath;
                if (_currentType == AssetType.Scenes)
                {
                    finalPath = Path.Combine(ProjectManager.ProjectRootPath,
                        ProjectManager.CurrentProject.SceneDirectory);
                }
                else
                {
                    finalPath = Path.Combine(ProjectManager.ProjectRootPath, ProjectManager.CurrentProject.AssetDirectory, _currentType.ToString());
                }

                if (Directory.Exists(finalPath))
                {
                    ImGui.BeginChild("AssetsList");
                    string[] files = Directory.GetFiles(finalPath);
                    foreach (string file in files)
                    {
                        string fileName = Path.GetFileName(file);
                        string fileNameWhithoutExtension = Path.GetFileNameWithoutExtension(file);
                        
                        if (fileName.StartsWith(".")) continue;
                        if (ImGui.Selectable(fileNameWhithoutExtension))
                        {
                            SelectedAsset = file;
                        }
            
                        if (_currentType == AssetType.Scenes)
                        {
                            SceneSerializer.Load(engine.CurrentScene, engine, file);
                            ProjectManager.CurrentSceneName = fileNameWhithoutExtension;
                            ProjectManager.CurrentScene = engine.CurrentScene;
                        }
                    }
                    ImGui.EndChild();
                }
            }
            
            ImGui.EndTabBar();
        }
    
        ImGui.End();
    }
}
