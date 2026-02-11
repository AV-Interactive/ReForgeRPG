using System.Text.Json;
using System.Text.Json.Serialization;

namespace ReForge.Engin.Core;

public class SaveSystem
{
    static string GetSavePath(int slot) => 
        Path.Combine(ProjectManager.ProjectRootPath, ProjectManager.CurrentProject.SavesDirectory, $"{slot}.save");
    
    public static void SaveGameState(int slot)
    {
        string dirSavePath = Path.Combine(ProjectManager.ProjectRootPath, ProjectManager.CurrentProject.SavesDirectory);
        Directory.CreateDirectory(dirSavePath);
        string savePath = GetSavePath(slot);
        var options = new JsonSerializerOptions {WriteIndented = true};
        string json = JsonSerializer.Serialize(ProjectManager.GameState, options);
        
        File.WriteAllText(savePath, json);
    }
    
    public static void LoadGameState(int slot)
    {
        string savePath = GetSavePath(slot);
        if (!File.Exists(savePath)) return;

        try
        {
            string json = File.ReadAllText(savePath);
            GameState? loadedData = JsonSerializer.Deserialize<GameState>(json);
            ProjectManager.SetGameState(loadedData);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to load save game: {e.Message}");
        }
    }
}
