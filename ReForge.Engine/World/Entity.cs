using System.Numerics;
using Raylib_cs;

namespace ReForge.Engine.World;

public class Entity
{
    public Vector2 Position;
    public Texture2D Texture;
    public string Name;

    public Entity(Vector2 position, Texture2D texture, string name)
    {
        Position = position;
        Texture = texture;
        Name = name;
    }

    public virtual void Draw()
    {
        Raylib.DrawTextureV(Texture, Position, Color.White);
    }
    
    public virtual void Update(float deltaTime)
    {
        
    }
}
