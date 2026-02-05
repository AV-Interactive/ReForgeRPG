using Raylib_cs;
namespace ReForge.Engine.World.Behaviors;

public class BoxCollider: Behavior
{
    public float Width { get; set; }
    public float Height { get; set; }

    public bool IsTrigger { get; set; } = false;
    
    public Action<Entity>? OnCollisionEnter;
    public Action<Entity>? OnCollisionStay;
    public Action<Entity>? OnCollisionExit;

    public Rectangle Bounds => new Rectangle(Owner.Position.X, Owner.Position.Y, Width, Height);
    
    public override void Update(float deltaTime)
    {
        if (Width == 0) Width = Owner.Texture.Width;
        if (Height == 0) Height = Owner.Texture.Height;
    }

    public void DrawDebug()
    {
        Raylib.DrawRectangleLinesEx(Bounds, 2, Color.Green);
    }
}
