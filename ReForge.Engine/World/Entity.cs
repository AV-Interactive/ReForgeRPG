using System.Numerics;
using Raylib_cs;

namespace ReForge.Engine.World;

public class Entity
{
    public Vector2 Position;
    public Texture2D Texture;
    public string Name;

    List<Behavior> _behaviors = new List<Behavior>();
    List<string> _tags = new List<string>();

    public Entity(Vector2 position, Texture2D texture, string name)
    {
        Position = position;
        Texture = texture;
        Name = name;
    }

    public void AddBehavior(Behavior behavior)
    {
        behavior.Owner = this;
        _behaviors.Add(behavior);
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
        var clone = new Entity(this.Position, this.Texture, this.Name + "_Copy");
        
        foreach (var tag in _tags) clone.AddTag(tag);

        foreach (var behavior in _behaviors)
        {
            var clonedBehavior = behavior.Clone();
            clone.AddBehavior(clonedBehavior);
        }
        
        return clone;
    }
    
    public void AddTag(string tag) => _tags.Add(tag);
    public bool HasTag(string tag) => _tags.Contains(tag);
    public void RemoveTag(string tag) => _tags.Remove(tag);
    public List<string> Tags => _tags;
}
