using ImGuiNET;
using rlImGui_cs;

namespace Reforge.Editor.UI;

public class ContentBrowser
{
    public string SelectedAsset { get; set; } = "";

    public void Draw()
    {
        ImGui.Begin("Content Browser");
        
        // Chemin vers tes assets (à adapter selon ton dossier sur Mac)
        string path = "Assets"; 
    
        if (Directory.Exists(path))
        {
            foreach (var file in Directory.GetFiles(path, "*.png"))
            {
                string fileName = Path.GetFileName(file);
                // On affiche le fichier. Si on clique, on le sélectionne.
                if (ImGui.Selectable(fileName, SelectedAsset == file))
                {
                    SelectedAsset = file;
                }
            }
        }
        else { ImGui.Text("Dossier Assets non trouvé !"); }
        
        ImGui.End();
    }
}
