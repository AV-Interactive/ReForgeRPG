using Raylib_cs;
using ReForge.Engine.World.Components;

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

    public override void Initialize()
    {
        /*OnCollisionEnter += (target) => Owner.BroadcastEvent("OnCollisionEnter", target);
        OnCollisionExit += (target) => Owner.BroadcastEvent("OnCollisionExit", target);*/
        OnCollisionEnter = (target) => Owner.BroadcastEvent("OnCollisionEnter", target);
        OnCollisionExit = (target) => Owner.BroadcastEvent("OnCollisionExit", target);
    }
    
    public override void Update(float deltaTime)
    {
        if (Width == 0 || Height == 0)
        {
            var sprite = Owner.GetBehavior<SpriteComponent>();
            if(sprite != null && sprite.Texture.Id != 0)
            {
                Width = sprite.Texture.Width;
                Height = sprite.Texture.Height;
            }
        }
    }

    public override Behavior Clone()
    {
        return new BoxCollider 
        { 
            Width = this.Width, 
            Height = this.Height, 
            IsTrigger = this.IsTrigger 
        };
    }

    public void DrawDebug()
    {
        Raylib.DrawRectangleLinesEx(Bounds, 2, Color.Green);
    }
}
