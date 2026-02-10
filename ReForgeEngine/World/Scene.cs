using Raylib_cs;
using ReForge.Engine.Physics;
using ReForge.Engine.World.Behaviors;

namespace ReForge.Engine.World;

public class Scene
{
    List<Entity> _entities = new List<Entity>();
    List<Entity> _entitiesToRemove = new List<Entity>();
    
    public void AddEntity(Entity entity)
    {
        _entities.Add(entity);
    }
    
    public void Update(float deltaTime)
    {
        foreach (Entity entity in _entities)
        {
            entity.Update(deltaTime);
        }

        CollisionSystem.Update(_entities);
        CleanUp();
    }
    
    public void Draw()
    {
        CleanUp();
        foreach (Entity entity in _entities.OrderBy(e => e.ZIndex))
        {
            entity.Draw();
        }
    }
    
    public void DestroyEntity(Entity entity)
    {
        _entitiesToRemove.Add(entity);
    }
    
    void CleanUp()
    {
        if (_entitiesToRemove.Count == 0) return;
        
        foreach (Entity entity in _entitiesToRemove)
        {
            _entities.Remove(entity);
        }
        _entitiesToRemove.Clear();
    }

    public Entity? GetActiveCameraEntity()
    {
        return _entities.FirstOrDefault(e => e.GetBehavior<CameraFollow>() != null);
    }
    
    public List<Entity> Entities => _entities;
}
