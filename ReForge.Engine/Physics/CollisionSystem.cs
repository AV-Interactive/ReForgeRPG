using Raylib_cs;
using ReForge.Engine.World;
using ReForge.Engine.World.Behaviors;

namespace ReForge.Engine.Physics;

public static class CollisionSystem
{
    static HashSet<(int, int)> _activeCollisions = new HashSet<(int, int)>();
    
    public static void Update(List<Entity> entities)
    {
        var collidables = entities.Where(e => e.GetBehavior<BoxCollider>() != null).ToList();
        HashSet<(int, int)> currentFramePosition = new HashSet<(int, int)>();
        
        for (int i = 0; i < collidables.Count; i++)
        {
            var entA = collidables[i];
            var colA = entA.GetBehavior<BoxCollider>();
            if (colA == null) continue;

            for (int j = i + 1; j < collidables.Count; j++)
            {
                var entB = collidables[j];
                var colB = entB.GetBehavior<BoxCollider>();
                if (colB == null) continue;

                if (Raylib.CheckCollisionRecs(colA.Bounds, colB.Bounds))
                {
                    var pair = (entA.GetHashCode(), entB.GetHashCode());
                    currentFramePosition.Add(pair);

                    if (!_activeCollisions.Contains(pair))
                    {
                        /* ENTER */
                        colA.OnCollisionEnter?.Invoke(entB);
                        colB.OnCollisionEnter?.Invoke(entA);
                    }
                    else
                    {
                        /* STAY */
                        colA.OnCollisionStay?.Invoke(entB);
                        colB.OnCollisionStay?.Invoke(entA);
                    }
                    
                    /* PHYSIC COLLISION */
                    if (!colA.IsTrigger && !colB.IsTrigger)
                    {
                        Rectangle overlap = Raylib.GetCollisionRec(colA.Bounds, colB.Bounds);
                        
                        if (overlap.Width < overlap.Height)
                        {
                            if(entA.Position.X < entB.Position.X) 
                                entA.Position.X -= overlap.Width;
                            else 
                                entA.Position.X += overlap.Width;
                        }
                        else
                        {
                            if(entA.Position.Y < entB.Position.Y) 
                                entA.Position.Y -= overlap.Height;
                            else 
                                entA.Position.Y += overlap.Height;
                        }
                    }
                }
            }
        }
        
        /* EXIT */
        foreach (var pair in _activeCollisions)
        {
            if (!currentFramePosition.Contains(pair))
            {
                var entA = entities.FirstOrDefault(e => e.GetHashCode() == pair.Item1);
                var entB = entities.FirstOrDefault(e => e.GetHashCode() == pair.Item2);
                
                entA?.GetBehavior<BoxCollider>()?.OnCollisionExit?.Invoke(entB);
                entB?.GetBehavior<BoxCollider>()?.OnCollisionExit?.Invoke(entA);
            }
        }
        _activeCollisions = currentFramePosition;
    }
}
