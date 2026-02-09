using System.Text.Json.Serialization;

namespace ReForge.Engine.World;

public abstract class Behavior
{
    [JsonIgnore]
    public Entity Owner { get; set; } = null!;
    
    public virtual void Initialize() {}
    public abstract void Update(float deltaTime);
    public abstract Behavior Clone();
    public virtual void OnReceivedEvent(string eventName, object? data = null) {}
}
