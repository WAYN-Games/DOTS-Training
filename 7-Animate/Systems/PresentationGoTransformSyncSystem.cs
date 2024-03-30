using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.VFX;

/// <summary>
/// This system keep the game object position in sync with its entity
/// We can't use burst in this system because we are iterating over managed components
/// </summary>
[UpdateAfter(typeof(TransformSystemGroup))]
[RequireMatchingQueriesForUpdate]
public partial struct PresentationGoTransformSyncSystem : ISystem
{

    public void OnUpdate(ref SystemState state)
    {
        foreach (var (localToWorld,transform) 
                 in SystemAPI.Query<RefRO<LocalToWorld>,SystemAPI.ManagedAPI.UnityEngineComponent<Transform>>())
        {
            // Once we have the managed component, we can use it normally to do anything we want
            transform.Value.position =  localToWorld.ValueRO.Position;
            transform.Value.rotation =  localToWorld.ValueRO.Rotation;
        }
    }

}