using System.Numerics;
using Raylib_cs;
using ReForge.Engine.Core;
using ReForge.Engine.World;
using ReForge.Engine.World.Behaviors;

namespace GameExample.Factories;

public static class EntityFactory
{
    public static Entity CreateActor(Engine engine, Vector2 position, string texPath, string name, List<string> tag)
    {
        var tex = engine.AssetManager.GetTexture(texPath);
        Entity playerEntity = new Entity(position, tex, name, texPath);
        
        foreach (var tagItem in tag) playerEntity.AddTag(tagItem);
        
        var collider = new BoxCollider();
        playerEntity.AddBehavior(collider);
        
        return playerEntity;
    }
}
