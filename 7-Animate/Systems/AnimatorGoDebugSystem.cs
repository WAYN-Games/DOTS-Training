using Unity.Collections;
using Unity.Entities;
using UnityEngine;using UnityEngine.EventSystems;

// We can't use burst in this system because we are iterating over managed components
[RequireMatchingQueriesForUpdate]
public partial struct AnimatorGoDebugSystem : ISystem
{

    public void OnUpdate(ref SystemState state)
    {
        foreach (var (animator, entity) 
                 in SystemAPI.Query<SystemAPI.ManagedAPI.UnityEngineComponent<Animator>>().WithAll<EnabledAnimatorLogging>().WithEntityAccess())
        {
            Debug.Log($"Entity {entity.Index}:{entity.Version} is playing {animator.Value.GetCurrentAnimatorClipInfo(0)[0].clip.name}");
        }
        
    }
}
 
