using System.Text.Json;
using ReForge.Engine.Core;
using ReForge.Engine.World;

namespace ReForge.Engin.Core;

public static class ProjectManager
{
    static string _configPath = Path.Combine(AppContext.BaseDirectory, "lastProjectPath.txt");
    public static ProjectSettings? CurrentProject { get; private set; }
    public static string CurrentSceneName { get; set; }
    public static Scene? CurrentScene { get; set; }
    public static string ProjectRootPath { get; private set; }
    public static bool IsSaved { get; set; } = false;

    public static bool LoadProject(string filePath)
    {
        if (!File.Exists(filePath)) return false;

        try
        {
            string json = File.ReadAllText(filePath);
            CurrentProject = JsonSerializer.Deserialize<ProjectSettings>(json);

            if (CurrentProject != null)
            {
                ProjectRootPath = Path.GetDirectoryName(filePath) ?? "";
                IsSaved = true;
                return true;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erreur lors du chargement du projet : {e.Message}");
        }
        return false;
    }
    
    public static void SaveLastProjectPath()
    {
        string path = Path.Combine(ProjectRootPath, CurrentProject.ProjectName + ".reforge");
        File.WriteAllText(_configPath, path);
    }

    public static bool TryLoadLastProject()
    {
        if (File.Exists(_configPath))
        {
            string path = File.ReadAllText(_configPath);
            return LoadProject(path);
        }
        return false;
    }

    public static void CreateEmptyTemporaryProject()
    {
        CurrentProject = new ProjectSettings
        {
            ProjectName = "Nouveau Projet",
            AssetDirectory = "Assets",
            StartScenePath = "Scenes/StartScene.scn"
        };
        
        ProjectRootPath = AppContext.BaseDirectory;
        
        Directory.CreateDirectory(Path.Combine(ProjectRootPath, CurrentProject.AssetDirectory));
        
        string assetsBase = Path.Combine(AppContext.BaseDirectory, CurrentProject.AssetDirectory);
        
        foreach (AssetType type in Enum.GetValues(typeof(AssetType)))
        {
            string dirPath;
            if (type == AssetType.Scenes)
            {
                dirPath = Path.Combine(assetsBase, type.ToString());
            }
            else
            {
                dirPath = Path.Combine(assetsBase, type.ToString());
            }
            Directory.CreateDirectory(dirPath);
        }
    }

    public static void SaveProject()
    {
        if (CurrentProject == null) return;

        try
        {
            string targetPath = ProjectRootPath;
            if (ProjectRootPath == AppContext.BaseDirectory)
            {
                targetPath = Path.Combine(AppContext.BaseDirectory, "Projects", CurrentProject.ProjectName);
            }
            
            Directory.CreateDirectory(targetPath);
            ProjectRootPath = targetPath;
            
            foreach (AssetType type in Enum.GetValues(typeof(AssetType)))
            {
                string dirPath;
                if (type == AssetType.Scenes)
                {
                    dirPath = Path.Combine(ProjectRootPath, CurrentProject.SceneDirectory);
                }
                else
                {
                    dirPath = Path.Combine(ProjectRootPath, CurrentProject.AssetDirectory, type.ToString());
                }
                Directory.CreateDirectory(dirPath);
            }
            
            string fullpath = Path.Combine(ProjectRootPath, $"{CurrentProject.ProjectName}.reforge");
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(CurrentProject, options);

            File.WriteAllText(fullpath,json);
            
            SaveLastProjectPath();
            
            IsSaved = true;
            Console.WriteLine($"Projet sauvegardé : {fullpath}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erreur lors de la sauvegarde du projet : {e.Message}");
        }
    }

    public static void SaveScene()
    {
        if (CurrentProject == null || string.IsNullOrEmpty(CurrentSceneName)) return;
        string scenePath = Path.Combine(ProjectRootPath, CurrentProject.SceneDirectory);
        Directory.CreateDirectory(scenePath);
        string fullPath = Path.Combine(scenePath, $"{CurrentSceneName}.scn");
        
        SceneSerializer.Save(CurrentScene, fullPath);
    }
}
