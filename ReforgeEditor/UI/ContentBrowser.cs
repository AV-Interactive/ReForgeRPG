using System.Numerics;
using ImGuiNET;
using Raylib_cs;
using Reforge.Editor.Core;
using ReForge.Engin.Core;
using ReForge.Engine.Core;
using ReForge.Engine.World;
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
                        if (_currentType != type)
                        {
                            _currentType = type;
                            SelectedAsset = "";                        
                        }
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
                    ImGui.BeginChild("AssetsList", new Vector2(0, ctx.BrowserContentHeight * 0.4f)); // On limite la hauteur de la liste
                    string[] files = Directory.GetFiles(finalPath);
                    foreach (string file in files)
                    {
                        string fileNameWhithoutExtension = Path.GetFileNameWithoutExtension(file);
        
                        if (Path.GetFileName(file).StartsWith(".")) continue;

                        bool isSelected = (SelectedAsset == file);
                        if (ImGui.Selectable(fileNameWhithoutExtension, isSelected))
                        {
                            // On stocke le chemin relatif aux assets si possible
                            string assetRoot = Path.Combine(ProjectManager.ProjectRootPath, ProjectManager.CurrentProject.AssetDirectory);
                            if (file.StartsWith(assetRoot))
                            {
                                SelectedAsset = Path.GetRelativePath(assetRoot, file);
                            }
                            else
                            {
                                SelectedAsset = file;
                            }
            
                            if (_currentType == AssetType.Scenes)
                            {
                                SceneSerializer.Load(engine.CurrentScene, engine, file);
                                ProjectManager.CurrentSceneName = fileNameWhithoutExtension;
                            }
                        }
                    }
                    ImGui.EndChild();

                    if ((_currentType == AssetType.Decors || _currentType == AssetType.Actors) && !string.IsNullOrEmpty(SelectedAsset))
                    {
                        ImGui.Separator();
                        ImGui.TextColored(new Vector4(1, 0.8f, 0, 1), "Tileset:");
                        DrawTileset(engine, ctx, SelectedAsset); // On l'appelle à chaque frame tant qu'un asset est sélectionné
                    }
                }
            }
            
            ImGui.EndTabBar();
        }
    
        ImGui.End();
    }

    void DrawTileset(Engine engine, EditorContext ctx, string assetPath)
    {
        string fullPath = assetPath;
        if (!Path.IsPathRooted(assetPath))
        {
            fullPath = Path.Combine(ProjectManager.ProjectRootPath, ProjectManager.CurrentProject.AssetDirectory, assetPath);
        }

        Texture2D texture = engine.AssetManager.GetTexture(fullPath);
        float tileSize = EditorConfig.TileSize;
        int cols = texture.Width / (int)tileSize;
        int rows = texture.Height / (int)tileSize;

        if (ImGui.BeginChild("TilesetPreview", new Vector2(0, 0)))
        {
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    Vector2 uv0 = new Vector2((x * tileSize) / texture.Width, (y * tileSize) / texture.Height);
                    Vector2 uv1 = new Vector2(((x + 1) * tileSize) / texture.Width, ((y + 1) * tileSize) / texture.Height);
                    
                    ImGui.PushID(y * cols + x);
                    if (ImGui.ImageButton($"##tile_{x}_{y}", (IntPtr)texture.Id, new Vector2(tileSize, tileSize), uv0, uv1))
                    {
                        ctx.SelectedTile = y * cols + x;
                    }
                    ImGui.PopID();
                    
                    if((x + 1)  < cols) ImGui.SameLine();
                }
            }
            ImGui.EndChild();
        }
    }
}
