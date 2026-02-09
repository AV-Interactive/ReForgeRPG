using System.Numerics;
using System.Text.Json.Serialization;
using Raylib_cs;
using ReForge.Engine.World.Behaviors;
using ReForge.Engine.World.Components;

namespace ReForge.Engine.World;

public class Entity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "Nouvel element";
    public Vector2 Position 
    {
        get => GetBehavior<TransformComponent>().Position;
        set => GetBehavior<TransformComponent>().Position = value;
    }
    public int ZIndex { get; set; } = 0;
    public string TexturePath { get; set; } = "";

    public List<string> Tags
    {
        get => _tags;
        set => _tags = value;
    }

    public List<Behavior> Behaviors
    {
        get => _behaviors;
        set => _behaviors = value;
    }

    public Entity()
    {
        AddBehavior(new TransformComponent());
    }
    
    public TransformComponent Transform => GetBehavior<TransformComponent>();

    [JsonIgnore]
    public Texture2D Texture { get; set; }
    
    List<Behavior> _behaviors = new List<Behavior>();
    List<string> _tags = new List<string>();

    public Entity(Vector2 position, Texture2D texture, string name, string texturePath) : this()
    {
        Position = position;
        Texture = texture;
        TexturePath = texturePath;
        Name = name;
    }

    public void AddBehavior(Behavior behavior)
    {
        behavior.Owner = this;
        
        if (behavior is BoxCollider && GetBehavior<ActionTrigger>() == null)
        {
            var trigger = new ActionTrigger();
            trigger.Owner = this;
            AddBehavior(trigger);
        }

        if (behavior is TransformComponent && this.GetBehavior<TransformComponent>() != null) return;
        
        _behaviors.Add(behavior);
        behavior.Initialize();
    }

    public void RemoveBehavior(Behavior behavior)
    {
        _behaviors.Remove(behavior);
    }

    public virtual void Draw()
    {
        Raylib.DrawTextureV(Texture, Position, Color.White);
    }

    public T? GetBehavior<T>() where T : Behavior
    {
        // On regarde si un comportement specifique existe dans la liste
        return _behaviors.OfType<T>().FirstOrDefault();
    }
    
    public virtual void Update(float deltaTime)
    {
        foreach (Behavior behavior in _behaviors)
        {
            behavior.Update(deltaTime);
        }
    }

    public Entity Clone()
    {
        var clone = new Entity(this.Position, this.Texture, this.Name, this.TexturePath);
        
        foreach (var tag in _tags) clone.AddTag(tag);

        foreach (var behavior in _behaviors)
        {
            var clonedBehavior = behavior.Clone();
            clone.AddBehavior(clonedBehavior);
        }
        
        return clone;
    }

    public void BroadcastEvent(string eventName, object data)
    {
        foreach (Behavior behavior in _behaviors)
        {
            behavior.OnReceivedEvent(eventName, data);
        }
    }
    
    public void AddTag(string tag) => _tags.Add(tag);
    public bool HasTag(string tag) => _tags.Contains(tag);
    public void RemoveTag(string tag) => _tags.Remove(tag);
}
