using System.Numerics;
using ImGuiNET;
using ReForge.Engine.Core;
using rlImGui_cs;

namespace Reforge.Editor.UI;

public class ContentBrowser
{
    public string SelectedAsset { get; set; } = "";

    public void Draw(Engine engine)
    {
        ImGui.Begin("Explorateur");
    
        string path = Path.Combine(AppContext.BaseDirectory, "Assets"); 
        string scenesPath = Path.Combine(path, "Scenes");

        if (Directory.Exists(path))
        {
            ImGui.TextColored(new Vector4(0, 1, 0, 1), "Assets");
            
            var pngFiles = Directory.GetFiles(path, "*.png");
            for (int i = 0; i < pngFiles.Length; i++)
            {
                string file = pngFiles[i];
                string fileName = Path.GetFileName(file);

                ImGui.PushID(i); 

                if (ImGui.Selectable(fileName, SelectedAsset == file))
                {
                    SelectedAsset = file;
                }

                ImGui.PopID(); 
            }
            
            ImGui.Separator();
            
            ImGui.TextColored(new Vector4(0, 1, 0, 1), "Scènes");

            if (Directory.Exists(scenesPath))
            {
                var sceneFiles = Directory.GetFiles(scenesPath, "*.json");
                for (int i = 0; i < sceneFiles.Length; i++)
                {
                    string file = sceneFiles[i];
                    string fileName = Path.GetFileName(file);

                    ImGui.PushID(i); 

                    if (ImGui.Selectable(fileName, SelectedAsset == file))
                    {
                        SelectedAsset = file;
                    }

                    if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                    {
                        SceneSerializer.Load(engine.CurrentScene, engine, file);
                    }

                    ImGui.PopID(); 
                }
            }
        }
        else { ImGui.Text($"Dossier Assets non trouvé !"); }
    
        ImGui.End();
    }
}
