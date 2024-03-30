using System;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// Entities using this component will have a GameObject counter part that can be interacted with from the monobehaviour or from a system.
/// </summary>
public class GameObjectCompanionAuthoring : MonoBehaviour
{
    [Tooltip("GameObject prefab that will be used for presentation")]
    public GameObject Prefab;

    [Header("Gizmo")] 
    public Color GizmoColor;
    
    public class GameObjectCompanionBaker : Baker<GameObjectCompanionAuthoring>
    {
        public override void Bake(GameObjectCompanionAuthoring authoring)
        {
            Entity bakingEntity = GetEntity(TransformUsageFlags.WorldSpace);

            // We store the game object prefab into a managed component
            var pgo = new PresentationGo
            {
                Prefab = authoring.Prefab
            };
            AddComponentObject(bakingEntity, pgo);
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = GizmoColor;
        Gizmos.DrawWireSphere(transform.position, 1);
    }
}