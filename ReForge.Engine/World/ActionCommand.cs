using System.Numerics;

namespace ReForge.Engine.World;

public enum ActionVerb
{
    Destroy,
    Teleport,
    ToggleActive,
    ChangeIndex
}

public class ActionCommand
{
    public ActionVerb Verb { get; set; }
    public string TargetTag { get; set; } = "";
    public bool TargetSelf { get; set; } = true;
    public Vector2 Destination { get; set; }
}
