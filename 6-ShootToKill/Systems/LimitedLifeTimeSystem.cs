using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;

[BurstCompile]
public partial struct LimitedLifeTimeSystem : ISystem
{
    private EntityQuery _query;
    
    private ComponentTypeHandle<LimitedLifeTime> _limitedLifeTimeTypeHandle;
    private EntityTypeHandle _entityTypeHandle;

    public void OnCreate(ref SystemState state)
    {
        
        // We need to declare the query to filter the entities to match
        _query = SystemAPI.QueryBuilder().WithAllRW<LimitedLifeTime>().Build();
        
        
        // Get an handle to the components we need to use in the job
        _limitedLifeTimeTypeHandle = state.GetComponentTypeHandle<LimitedLifeTime>(false);
        _entityTypeHandle = state.GetEntityTypeHandle();
        
        
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate(_query);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        
        var ecbBos = SystemAPI
            .GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        // Update the handles before executing hte job
        _limitedLifeTimeTypeHandle.Update(ref state);
        _entityTypeHandle.Update(ref state);

        // declare the job
        state.Dependency = new LimitedLifeTimeJob()
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            LimitedLifeTimeTypeHandle = _limitedLifeTimeTypeHandle,
            EcbBos = ecbBos.AsParallelWriter(),
            EntityTypeHandle = _entityTypeHandle
        }
            // Schedule the job
            .ScheduleParallel(_query, state.Dependency);
       
    }

    public struct LimitedLifeTimeJob : IJobChunk
    {
        [ReadOnly] public EntityTypeHandle EntityTypeHandle;
        public ComponentTypeHandle<LimitedLifeTime> LimitedLifeTimeTypeHandle;
        
        public EntityCommandBuffer.ParallelWriter EcbBos;
        
        public float DeltaTime;
        
        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            // Get the array of components the job instance can process
            NativeArray<LimitedLifeTime> limitedLifeTimeArray = chunk.GetNativeArray(ref LimitedLifeTimeTypeHandle);
            NativeArray<Entity> entityArray = chunk.GetNativeArray(EntityTypeHandle);
            
            // loop through all the entities managed by the chunk
            for (int i = 0; i < chunk.Count; i++)
            {
                
                // perform our work
                LimitedLifeTime limitedLifeTime = limitedLifeTimeArray[i];
                limitedLifeTime.TimeRemaining -= DeltaTime;
                limitedLifeTimeArray[i] = limitedLifeTime;
                
                if(limitedLifeTime.TimeRemaining < 0)
                {
                    EcbBos.DestroyEntity(unfilteredChunkIndex,entityArray[i]);
                }
            }

        }
    } 
    
}
