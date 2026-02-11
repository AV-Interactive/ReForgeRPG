namespace ReForge.Engin.Core;

public class GameState
{
    public Dictionary<string, bool> Switches { get; set; } = new();
    public Dictionary<string, float> Variables { get; set; } = new();
}
