using System.Numerics;
using ReForge.Engine.World;

namespace ReForge.Engine.World.Behaviors;

public class CameraFollow: Behavior
{
    public Entity Target { get; set; }
    public float Smoothness { get; set; } = 5.0f;
    
    public override void Update(float deltaTime)
    {
        if (Target == null)
        {
            Console.WriteLine("No target found");
            Target = Core.Engine.Instance.CurrentScene.Entities
                .FirstOrDefault(e => e.Tags.Any(t => t.Trim().Equals("Player", StringComparison.InvariantCultureIgnoreCase)));
        }
        
        if(Target == null) return;
        
        Owner.Position = Vector2.Lerp(Owner.Position, Target.Position, Smoothness * deltaTime);
    }

    public override Behavior Clone()
    {
        return new CameraFollow
        {
            Target = this.Target,
            Smoothness = this.Smoothness
        };
    }
}
