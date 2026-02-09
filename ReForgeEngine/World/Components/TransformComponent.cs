using System.Numerics;

namespace ReForge.Engine.World.Components;

public class TransformComponent: Behavior
{
    public Vector2 Position { get; set; }
    
    public override void Update(float deltaTime)
    {
        //
    }

    public override Behavior Clone()
    {
        return new TransformComponent()
        {
            Position = this.Position
        };
    }
}
