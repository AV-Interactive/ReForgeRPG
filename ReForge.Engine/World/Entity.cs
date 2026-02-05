using System.Numerics;
using Raylib_cs;

namespace ReForge.Engine.World;

public class Entity
{
    public Vector2 Position;
    public Texture2D Texture;
    public string Name;

    List<Behavior> _behaviors = new List<Behavior>();

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
    
    public virtual void Update(float deltaTime)
    {
        foreach (Behavior behavior in _behaviors)
        {
            behavior.Update(deltaTime);
        }
    }
}
