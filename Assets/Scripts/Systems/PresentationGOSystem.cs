using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial struct PresentationGOSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecbBOS = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach(var (pgo,entity) in SystemAPI.Query<PresentationGO>().WithEntityAccess())
        {
            GameObject go = GameObject.Instantiate(pgo.Prefab);
            go.AddComponent<EntityGameObject>().AssignEntity(entity, state.World);

            ecbBOS.AddComponent(entity, new TransformGO() { Transform = go.transform });
            ecbBOS.AddComponent(entity, new AnimatorGO() { Animator = go.GetComponent<Animator>() });

            ecbBOS.RemoveComponent<PresentationGO>(entity);
        }
        var ecbEOS = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (goTransform,goAnimator,tranform,speed) in SystemAPI.Query<TransformGO, AnimatorGO, TransformAspect, RefRO<Speed>>())
        {
            goTransform.Transform.position = tranform.Position;
            goTransform.Transform.rotation = tranform.Rotation;
            goAnimator.Animator.SetFloat("speed", speed.ValueRO.value);
        }
        foreach(var (goTransform,entity) in SystemAPI.Query<TransformGO>().WithNone<LocalToWorld>().WithEntityAccess())
        {
            if (goTransform.Transform != null)
            {
                GameObject.Destroy(goTransform.Transform.gameObject);
            }
            ecbEOS.RemoveComponent<TransformGO>(entity);
        }
    }
}