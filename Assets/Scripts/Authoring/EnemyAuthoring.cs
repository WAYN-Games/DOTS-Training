using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    public float Speed;
    public GameObject Prefab;
}

public class PresentationGO : IComponentData
{
    public GameObject Prefab;
}
public class TransformGO : ICleanupComponentData
{
    public Transform Transform;
}

public class AnimatorGO : IComponentData
{
    public Animator Animator;
}


public class EnemyBaker : Baker<EnemyAuthoring>
{
    public override void Bake(EnemyAuthoring authoring)
    {
        if (authoring.Speed > 0)
        {
            Speed speed = default;
            speed.value = authoring.Speed;
            AddComponent(speed);
        }

        PresentationGO pgo = new PresentationGO();
        pgo.Prefab = authoring.Prefab;
        AddComponentObject(pgo);
    }
}

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
            go.AddComponent<EntityGameObject>().AssignEntity(entity, state.EntityManager);

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
            GameObject.Destroy(goTransform.Transform.gameObject);
            ecbEOS.RemoveComponent<TransformGO>(entity);
        }
    }
}