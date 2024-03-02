using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

/// <summary>
/// This system will manage the spawning of entities at a given rate.
/// The Attribute UpdateAfter allows us to specify the execution order of the system.
/// Here we want the system to update after the TransformSystemGroup in order to make use of the up to date LocalToWorld data.
/// (
/// In reality, since the spawner does not move, the LocalToWorld data will remain the same every frame so it would not matter
/// But if you want to work with LocalToWorld data, keep in mind that systems executing before the TransformSystemGroup will have the LocalToWorld of the previous frame
/// and systems executing after the TransformSystemGroup will have the up to date LocalToWorld 
/// )
/// </summary>
[UpdateAfter(typeof(TransformSystemGroup))]
[BurstCompile]
public partial struct SpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // The system make use of an EntityCommandBuffer, therefore, it needs the system handling the entity command buffer to be initialized to run 
        state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // We first create an Entity Command Buffer that will allows us to queue the instantiation command
        // We use the BeginSimulationEntityCommandBufferSystem's Command Buffer to delay the instantiation of the entity to the beginning of the simulation loop of the next frame.  
        // The built in command buffers are :
        //      BeginInitializationEntityCommandBufferSystem
        //      EndInitializationEntityCommandBufferSystem
        //      BeginSimulationEntityCommandBufferSystem
        //      BeginFixedStepSimulationEntityCommandBufferSystem
        //      EndFixedStepSimulationEntityCommandBufferSystem
        //      BeginVariableRateSimulationEntityCommandBufferSystem
        //      EndVariableRateSimulationEntityCommandBufferSystem
        //      EndSimulationEntityCommandBufferSystem
        //      BeginPresentationEntityCommandBufferSystem
        EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);


        
        foreach(var (spawnerRW,positionRO,waypoints) in SystemAPI.Query<RefRW<SpawnerData>,RefRO<LocalToWorld>,DynamicBuffer<Waypoints>>()
                    
                    // Simulate is a Tag component, tag component are component that don't hold any data, they exists just to mark an entity, the most common example usage for it is state management
                    // However, adding and removing component even if they don't hold any data, trigger a structural change. that structural change is costly because it involves moving the entity's data
                    // This can be worked around by implementing the IEnableableComponent like this component does
                    // An IEnableableComponent can be active or inactive and change this state without structural change. 
                    .WithAll<Simulate>()) 
        {
            ref SpawnerData spawner = ref spawnerRW.ValueRW;
            ref readonly LocalToWorld position = ref positionRO.ValueRO;
            
            // Reduce time to next spawn
            spawner.TimeToNextSpawn -= SystemAPI.Time.DeltaTime;

            // If time to spawn a new entity has not been reached skipp the rest of the loop
            if (!(spawner.TimeToNextSpawn < 0)) continue;
            
            // Reset timer
            spawner.TimeToNextSpawn = spawner.SpawnInterval;
            
            // Spawn the entity at the spawner location
            Entity e = ecb.Instantiate(spawner.EnemyPrefab);
            ecb.SetComponent(e, LocalTransform.FromPosition(position.Position));
            
            // Copy the Waypoints from the spawner to the spawned entity 
            DynamicBuffer<Waypoints> buffer = ecb.AddBuffer<Waypoints>(e);
            buffer.AddRange(waypoints.AsNativeArray());
        }
    }
}
