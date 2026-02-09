using System.Numerics;

namespace ReForge.Engine.World.Behaviors;

public class Velocity : Behavior
{
    public Vector2 Current { get; set; } = Vector2.Zero;
    public float Friction { get; set; } = 0.95f;
    public override void Update(float deltaTime)
    {
        Owner.Position += Current * deltaTime;
        Current *= Friction;
    }

    public override Behavior Clone()
    {
        return new Velocity
        {
            Current = this.Current,
            Friction = this.Friction
        };
    }
}
