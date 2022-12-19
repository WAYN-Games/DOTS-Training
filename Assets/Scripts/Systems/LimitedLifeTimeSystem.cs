using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct LimitedLifeTimeSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbBOS = SystemAPI
            .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (time,entity) 
            in SystemAPI.Query<RefRW<LimitedLifeTime>>().WithEntityAccess())
        {
            time.ValueRW.TimeRemaining -= SystemAPI.Time.DeltaTime;
            if(time.ValueRO.TimeRemaining < 0)
            {
                ecbBOS.DestroyEntity(entity);
            }
        }
    }
}