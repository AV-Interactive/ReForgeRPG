using ImGuiNET;
using rlImGui_cs;

namespace Reforge.Editor.UI;

public class ContentBrowser
{
    public string SelectedAsset { get; set; } = "";

    public void Draw()
    {
        ImGui.Begin("Content Browser");
    
        string path = "Assets"; 

        if (Directory.Exists(path))
        {
            var files = Directory.GetFiles(path, "*.png");
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                string fileName = Path.GetFileName(file);

                ImGui.PushID(i); 

                if (ImGui.Selectable(fileName, SelectedAsset == file))
                {
                    SelectedAsset = file;
                }

                ImGui.PopID(); 
            }
        }
        else { ImGui.Text($"Dossier Assets non trouvé !"); }
    
        ImGui.End();
    }
}
