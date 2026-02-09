using System.Text.Json.Serialization;
using Raylib_cs;
using ReForge.Engine.Core;

namespace ReForge.Engine.World.Components;

[HiddenBehavior]
public class SpriteComponent: Behavior
{
    public string TexturePath { get; set; } = "";
    
    [JsonIgnore]
    public Texture2D Texture { get; set; }

    public override void Update(float deltaTime)
    {
        //
    }

    public override Behavior Clone()
    {
        return new SpriteComponent()
        {
            TexturePath = this.TexturePath,
            Texture = this.Texture
        };
    }
}
