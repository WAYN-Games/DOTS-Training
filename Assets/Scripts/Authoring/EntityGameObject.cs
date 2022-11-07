using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EntityGameObject : MonoBehaviour
{
    private Entity Entity;
    private EntityManager EntityManager;

    public void AssignEntity(Entity e, EntityManager em)
    {
        Entity = e;
        EntityManager = em;
    }

    public void OnDestroy()
    {
        if (EntityManager != null)
        {
            EntityManager.DestroyEntity(Entity);
        }
    }
}
