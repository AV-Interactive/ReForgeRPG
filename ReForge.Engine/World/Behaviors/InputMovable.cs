using System.Numerics;
using Raylib_cs;

namespace ReForge.Engine.World.Behaviors;

public class InputMovable: Behavior
{
    public float Speed { get; set; } = 250f;
    
    public override void Update(float deltaTime)
    {
        Vector2 currentPos = Owner.Position;
        
        if (Raylib.IsKeyDown(KeyboardKey.Right)) currentPos.X += Speed * deltaTime;
        if (Raylib.IsKeyDown(KeyboardKey.Left)) currentPos.X -= Speed * deltaTime;
        if (Raylib.IsKeyDown(KeyboardKey.Up)) currentPos.Y -= Speed * deltaTime;
        if (Raylib.IsKeyDown(KeyboardKey.Down)) currentPos.Y += Speed * deltaTime;
    }

    public override Behavior Clone()
    {
        return new InputMovable { Speed = this.Speed };
    }
}
