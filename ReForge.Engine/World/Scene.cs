using Raylib_cs;
using ReForge.Engine.Physics;

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
        foreach (Entity entity in _entities)
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
    
    public List<Entity> Entities => _entities;
}
