
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
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
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

        foreach (var (speed, target, transform, entity) in SystemAPI.Query<RefRO<Speed>, RefRO<Target>, RefRW<LocalTransform>>().WithEntityAccess())
        {
            if (positionLookup.HasComponent(target.ValueRO.Value))
            {
                if (!SystemAPI.HasBuffer<HitList>(entity))
                    transform.ValueRW.Rotation = TransformHelpers.LookAtRotation(transform.ValueRO.Position,positionLookup[target.ValueRO.Value].Position,transform.ValueRW.Up());
                    
                transform.ValueRW.Position = transform.ValueRO.Position + speed.ValueRO.value * SystemAPI.Time.DeltaTime * transform.ValueRO.Forward();
            }
            else
            {
                ecbBOS.DestroyEntity(entity);
            }
        }

    }
}
