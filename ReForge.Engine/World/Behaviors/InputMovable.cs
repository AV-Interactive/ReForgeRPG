using Raylib_cs;

namespace ReForge.Engine.World.Behaviors;

public class InputMovable: Behavior
{
    public float Speed { get; set; } = 250f;
    
    public override void Update(float deltaTime)
    {
        if (Raylib.IsKeyDown(KeyboardKey.Right)) Owner.Position.X += Speed * deltaTime;
        if (Raylib.IsKeyDown(KeyboardKey.Left)) Owner.Position.X -= Speed * deltaTime;
        if (Raylib.IsKeyDown(KeyboardKey.Up)) Owner.Position.Y -= Speed * deltaTime;
        if (Raylib.IsKeyDown(KeyboardKey.Down)) Owner.Position.Y += Speed * deltaTime;
    }
}
