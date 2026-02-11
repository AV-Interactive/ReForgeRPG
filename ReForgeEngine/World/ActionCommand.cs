using System.ComponentModel;
using System.Numerics;

namespace ReForge.Engine.World;

public enum ActionVerb
{
    [Description("Détruire l'objet")]
    Destroy,
    [Description("Téléporter l'objet")]
    Teleport,
    [Description("Activer/désactiver l'objet")]
    ToggleActive,
    [Description("Changer l'index de l'objet")]
    ChangeIndex,
    [Description("Activer/désactiver un interrupteur")]
    SetSwitch,
    [Description("Définir une variable")]
    SetVariable,
    [Description("Augmenter la valeur d'une variable")]
    AddValueVariable,
    [Description("Diminuer la valeur d'une variable")]
    SubtractValueVariable,
}

public class ActionCommand
{
    public ActionVerb Verb { get; set; }
    public string TargetTag { get; set; } = "";
    public string Key { get; set; } = "";
    public float Value { get; set; }
    public bool TargetSelf { get; set; } = false;
    public Vector2 Destination { get; set; }
}
