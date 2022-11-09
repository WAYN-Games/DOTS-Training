using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EntityGameObject : MonoBehaviour
{
    private Entity Entity;
    private World World;

    public void AssignEntity(Entity e, World world)
    {
        Entity = e;
        World = world;
    }

    public void OnDestroy()
    {
        if (World.IsCreated && World.EntityManager.Exists(Entity))
        {
            World.EntityManager.DestroyEntity(Entity);
        }
    }
}
