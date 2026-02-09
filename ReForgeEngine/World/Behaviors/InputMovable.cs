using System.Numerics;
using Raylib_cs;

namespace ReForge.Engine.World.Behaviors;

public class InputMovable: Behavior
{
    public float Speed { get; set; } = 250f;
    public float Acceleration { get; set; } = 50f;
    
    public override void Update(float deltaTime)
    {
        var velocity = Owner.GetBehavior<Velocity>();
        
        Vector2 direction = velocity.Current;
        if (Raylib.IsKeyDown(KeyboardKey.Right)) direction.X += Speed * deltaTime;
        if (Raylib.IsKeyDown(KeyboardKey.Left)) direction.X -= Speed * deltaTime;
        if (Raylib.IsKeyDown(KeyboardKey.Up)) direction.Y -= Speed * deltaTime;
        if (Raylib.IsKeyDown(KeyboardKey.Down)) direction.Y += Speed * deltaTime;

        if (direction != Vector2.Zero)
        {
            if (velocity != null)
            {
                velocity.Current += direction * Acceleration;
            }
            else
            {
                Owner.Position += direction * Speed * deltaTime;
            }
        }
    }

    public override Behavior Clone()
    {
        return new InputMovable { Speed = this.Speed };
    }
}
