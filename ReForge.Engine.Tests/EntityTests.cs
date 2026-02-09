using ReForge.Engine.World;
using ReForge.Engine.World.Components;
using System.Numerics;

namespace ReForge.Engine.Tests;

public class EntityTests
{
    [Fact]
    public void NewEntity_ShouldHaveTransformComponent()
    {
        // Arrange & Act
        var entity = new Entity();

        // Assert
        Assert.NotNull(entity.GetBehavior<TransformComponent>());
    }

    [Fact]
    public void AddTag_ShouldWork()
    {
        // Arrange
        var entity = new Entity();
        var tag = "Player";

        // Act
        entity.AddTag(tag);

        // Assert
        Assert.True(entity.HasTag(tag));
    }

    [Fact]
    public void RemoveTag_ShouldWork()
    {
        // Arrange
        var entity = new Entity();
        var tag = "Enemy";
        entity.AddTag(tag);

        // Act
        entity.RemoveTag(tag);

        // Assert
        Assert.False(entity.HasTag(tag));
    }

    [Fact]
    public void Position_ShouldUpdateTransform()
    {
        // Arrange
        var entity = new Entity();
        var newPos = new Vector2(10, 20);

        // Act
        entity.Position = newPos;

        // Assert
        Assert.Equal(newPos, entity.Position);
        Assert.Equal(newPos, entity.GetBehavior<TransformComponent>().Position);
    }
}
