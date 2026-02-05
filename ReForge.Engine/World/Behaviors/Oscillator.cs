using System.Numerics;

namespace ReForge.Engine.World.Behaviors;

public class Oscillator: Behavior
{
    public Vector2 Direction { get; set; } = new Vector2(1, 0);
    public float Distance { get; set; } = 100f;
    public float Speed { get; set; } = 2f;
    
    float _timer = 0f;
    Vector2 _startPosition;
    bool _initialized = false;
    
    
    public override void Update(float deltaTime)
    {
        if (!_initialized)
        {
            _startPosition = Owner.Position;
            _initialized = true;
        }
        
        _timer += deltaTime;
        
        float offset = (float)Math.Sin(_timer) * Distance;
        
        Owner.Position = _startPosition + (Direction * offset);
    }

    public override Behavior Clone()
    {
        return new Oscillator 
        { 
            Direction = this.Direction, 
            Distance = this.Distance, 
            Speed = this.Speed 
        };
    }
}
