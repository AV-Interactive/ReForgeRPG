namespace ReForge.Engine.World.Behaviors;

public class Timer: Behavior
{
    public float Duration { get; set; }
    float _elapsedTime;


    public override void Update(float deltaTime)
    {
        _elapsedTime += deltaTime;

        if (_elapsedTime >= Duration)
        {
            Core.Engine.Instance.CurrentScene.DestroyEntity(Owner);
        }
    }

    public override Behavior Clone()
    {
        return new Timer { Duration = this.Duration };
    }
}
