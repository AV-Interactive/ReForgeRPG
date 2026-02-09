using System.Numerics;
using Raylib_cs;

namespace ReForge.Engine.World.Behaviors;

public class InputMovable: Behavior
{
    public float Speed { get; set; } = 250f;
    
    public override void Update(float deltaTime)
    {
        Vector2 nextPos = Owner.Position;
        
        if (Raylib.IsKeyDown(KeyboardKey.Right)) nextPos.X += Speed * deltaTime;
        if (Raylib.IsKeyDown(KeyboardKey.Left)) nextPos.X -= Speed * deltaTime;
        if (Raylib.IsKeyDown(KeyboardKey.Up)) nextPos.Y -= Speed * deltaTime;
        if (Raylib.IsKeyDown(KeyboardKey.Down)) nextPos.Y += Speed * deltaTime;
        
        Owner.Position = nextPos;
    }

    public override Behavior Clone()
    {
        return new InputMovable { Speed = this.Speed };
    }
}
