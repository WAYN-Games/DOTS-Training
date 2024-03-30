using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct PresentationGoCleanupSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
    }
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecbBis = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        foreach(var (presentationGoCleanup,entity) 
                in SystemAPI.Query<PresentationGoCleanup>().WithNone<PresentationGo>().WithEntityAccess())
        {
            if (presentationGoCleanup.Instance == null) continue;
            // We try to get a DestructionManager from the game object
            var destructionManager = presentationGoCleanup.Instance.GetComponent<DestructionManager>();
            if (destructionManager != null)
            {
                // if it has one, we let it handle the destruction of the game object
                destructionManager.Destroy();
            }
            else
            {
                // If not we destroy it immediately
                Object.Destroy(presentationGoCleanup.Instance.gameObject);
            }
            // We remove the cleanup component to finalize the destruction of the entity
            ecbBis.RemoveComponent<PresentationGoCleanup>(entity);
        }
    }
}