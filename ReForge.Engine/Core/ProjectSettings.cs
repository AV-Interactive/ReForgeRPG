namespace ReForge.Engine.Core;

public class ProjectSettings
{
    public string ProjectName { get; set; } = "Untitled";
    public string AssetDirectory { get; set; } = "Assets";
    public string SceneDirectory { get; set; } = "Scenes";
    public string StartScenePath { get; set; }

    public string GetFullPath(string relativePath)
    {
        return Path.Combine(AssetDirectory, relativePath);
    }
}
