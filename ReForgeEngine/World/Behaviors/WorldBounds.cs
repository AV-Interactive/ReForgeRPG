using System.Numerics;
using ReForge.Engine.World.Components;

namespace ReForge.Engine.World.Behaviors;

public class WorldBounds: Behavior
{
    public Vector2 MinBounds { get; set; } = new Vector2(float.MinValue, float.MinValue);
    public Vector2 MaxBounds { get; set; } = new Vector2(float.MaxValue, float.MaxValue);

    public WorldBounds()
    {
        // S'assurer que le clamp s'applique après les déplacements
        UpdateOrder = 1000;
    }
    
    public override void Update(float deltaTime)
    {
        // Si les limites sont aux valeurs par défaut (infinies), on ne bloque rien
        if (MinBounds.X <= float.MinValue + 1 && MaxBounds.X >= float.MaxValue - 1)
        {
            return;
        }

        Vector2 position = Owner.Position;
        Vector2 maxEffective = MaxBounds;
        
        var sprite = Owner.GetBehavior<SpriteComponent>();
        if (sprite != null && sprite.Texture.Id != 0)
        {
            // On réduit la zone de mouvement autorisée de la taille du sprite
            maxEffective.X -= sprite.Texture.Width;
            maxEffective.Y -= sprite.Texture.Height;
        }
        
        position.X = Math.Clamp(position.X, MinBounds.X, maxEffective.X);
        position.Y = Math.Clamp(position.Y, MinBounds.Y, maxEffective.Y);
        
        Owner.Position = position;
    }

    public override Behavior Clone()
    {
        return new WorldBounds
        {
            MinBounds = this.MinBounds,
            MaxBounds = this.MaxBounds,
            UpdateOrder = this.UpdateOrder
        };
    }
}
