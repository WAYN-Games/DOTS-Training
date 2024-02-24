using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/// <summary>
/// This system handled the movement of an entity along a path.
/// </summary>
public partial struct PathFollowSystem : ISystem
{

    // This attribute means that the code will be compiled using the Burst compiler
    // The burst compiler enable great performance improvement applying some 
    // optimization automatically such as loop vectorization using SIMD instructions
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // To query a dynamic buffer you can use RefRO or RefRW, you just have to reference it directly.
        // Note that this implies the dynamic buffer is always accessed as Read/Write !
        foreach (var (speedRO,transformRW,nextPathIndexRW,waypoints) in SystemAPI.Query<RefRO<Speed>, RefRW<LocalTransform>,RefRW<NextPathIndex>,DynamicBuffer<Waypoints>>())
        {
            // This is a different syntax to avoid repeating ValueRO and ValueRW everytime, making the code a bit cleaner
            ref readonly Speed speed = ref speedRO.ValueRO;
            ref LocalTransform transform = ref transformRW.ValueRW;
            ref NextPathIndex nextPathIndex = ref nextPathIndexRW.ValueRW;
            
            // Check if the entity reach the waypoint
            if (math.distance(transform.Position, waypoints[nextPathIndex.value].Value) < 0.1f)
            {
                // set the destination to the next waypoint, circling back to the first waypoint if this was the last in the path.
                nextPathIndex.value = (nextPathIndex.value + 1) % waypoints.Length;
            }
            
            // Move the entity toward the next waypoint
            float3 direction = waypoints[nextPathIndex.value].Value - transform.Position;
            transform.Position += math.normalize(direction) * SystemAPI.Time.DeltaTime * speed.value;
            transform.Rotation = TransformHelpers.LookAtRotation(transform.Position,waypoints[nextPathIndex.value].Value,transform.Up());
        }
    }
}