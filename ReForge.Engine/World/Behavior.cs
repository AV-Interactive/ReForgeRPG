using System.Text.Json.Serialization;

namespace ReForge.Engine.World;

public abstract class Behavior
{
    [JsonIgnore]
    public Entity Owner { get; set; } = null!;
    
    public abstract void Update(float deltaTime);
    public abstract Behavior Clone();
}
