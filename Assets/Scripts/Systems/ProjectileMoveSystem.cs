
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateAfter(typeof(TransformSystemGroup))]
[BurstCompile]
public partial struct ProjectileMoveSystem : ISystem
{
    ComponentLookup<LocalTransform> positionLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        positionLookup = SystemAPI.GetComponentLookup<LocalTransform>(true);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbBOS = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        positionLookup.Update(ref state);

        foreach (var (speed, target, transform, entity) in SystemAPI.Query<RefRO<Speed>, RefRO<Target>, TransformAspect>().WithEntityAccess())
        {
            if (positionLookup.HasComponent(target.ValueRO.Value))
            {
                if (!SystemAPI.HasBuffer<HitList>(entity))
                    transform.LookAt(positionLookup[target.ValueRO.Value].Position);

                transform.WorldPosition = transform.WorldPosition + speed.ValueRO.value * SystemAPI.Time.DeltaTime * transform.Forward;
            }
            else
            {
                ecbBOS.DestroyEntity(entity);
            }
        }

    }
}
