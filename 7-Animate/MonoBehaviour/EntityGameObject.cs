using Unity.Entities;
using UnityEngine;


/// <summary>
/// This monobehaviour is automatically added to the game object if it is linked to an entity.
/// It allows to keep track of it's entity counter part.
/// </summary>
public class EntityGameObject : MonoBehaviour
{
    public Entity Entity;
    public World World;

    public void AssignEntity(Entity e, World world)
    {
        Entity = e;
        World = world;
    }

    // if the game object gets destroyed, we should also make sure qe destroy the entity counter part
    // if we don't we will have errors in the systems that uses the managed component referencing this destroyed game object
    public void OnDestroy()
    {
        if (World.IsCreated && World.EntityManager.Exists(Entity))
        {
            World.EntityManager.DestroyEntity(Entity);
        }
    }
}
