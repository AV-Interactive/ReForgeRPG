using System.Numerics;
using Raylib_cs;
using ReForge.Engine.World;

namespace ReForge.Engine.World.Behaviors;

public class CameraFollow: Behavior
{
    public Entity Target { get; set; }
    public float Smoothness { get; set; } = 5.0f;
    
    public Vector2 MinBounds { get; set; } = new Vector2(float.MinValue, float.MinValue);
    public Vector2 MaxBounds { get; set; } = new Vector2(float.MaxValue, float.MaxValue);
    
    public override void Update(float deltaTime)
    {
        if (Target == null)
        {
            Console.WriteLine("No target found");
            Target = Core.Engine.Instance.CurrentScene.Entities
                .FirstOrDefault(e => e.Tags.Any(t => t.Trim().Equals("Player", StringComparison.InvariantCultureIgnoreCase)));
        }
        
        if(Target == null) return;
        
        Vector2 nextPos = Vector2.Lerp(Owner.Position, Target.Position, Smoothness * deltaTime);
        
        float screenWidth = Raylib.GetScreenWidth();
        float screenHeight = Raylib.GetScreenHeight();
        
        float minX = MinBounds.X + (screenWidth / 2);
        float maxX = MaxBounds.X - (screenWidth / 2);
        float minY = MinBounds.Y + (screenHeight / 2);
        float maxY = MaxBounds.Y - (screenHeight / 2);

        if (maxX >= minX)
        {
            nextPos.X = Math.Clamp(nextPos.X, minX, maxX);
        }
        else
        {
            nextPos.X = MinBounds.X + (MaxBounds.X - MinBounds.X) / 2;
        }

        if (maxY >= minY)
        {
            nextPos.Y = Math.Clamp(nextPos.Y, minY, maxY);
        }
        else
        {
            nextPos.Y = MinBounds.Y + (MaxBounds.Y - MinBounds.Y) / 2;
        }
        
        Owner.Position = nextPos;
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
