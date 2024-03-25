using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct KillSystem : ISystem
{
    // An entity query is used to filter entities that match an archetype (set of component) definition
    private EntityQuery _query;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecbBss = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        
        state.Dependency = new ProjectileMoveJob()
        {
            EcsBss = ecbBss.AsParallelWriter() // since we use a parallel scheduling we need the parallel write version of the entity command buffer 
        }.ScheduleParallel(state.Dependency);
    }

    public partial struct ProjectileMoveJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter EcsBss;
        public void Execute([ChunkIndexInQuery] int chunkIndexInQuery, in Entity entity, in Health health)
        {
            if (health.Current > 0) return;
            // To queue command in parallel and have them played back in a deterministic way, we need to provide a sorting key
            // The best sorting key to use is the chunkIndexInQuery that can be accessed with the attribute [ChunkIndexInQuery]
            EcsBss.DestroyEntity(chunkIndexInQuery,entity);
        }
    }
}