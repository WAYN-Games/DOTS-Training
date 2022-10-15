using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;

public partial class Move : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = World.Time.DeltaTime;
        
        Entities.WithAll<RenderBounds>().ForEach((ref TransformAspect transform,ref NextPathIndex pathIndex,
            in DynamicBuffer<Waypoints> path, in Speed speed) => {
                float3 direction = path[pathIndex.value].value - transform.Position;
                if(math.distance(transform.Position, path[pathIndex.value].value) < 0.1f)
                {
                    pathIndex.value = (pathIndex.value + 1) % path.Length;
                }
                transform.Position += math.normalize(direction) * deltaTime* speed.value;
        }).Schedule();
        

    }
}


[BurstCompile]
public partial struct MoveIstsyem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (transform, pathIndex, path, speed) in SystemAPI.Query<TransformAspect, RefRW<NextPathIndex>, DynamicBuffer<Waypoints>, RefRO<Speed>>().WithAll<RenderBounds>())
        {

            float3 direction = path[pathIndex.ValueRO.value].value - transform.Position;
            if (math.distance(transform.Position, path[pathIndex.ValueRO.value].value) < 0.1f)
            {
                pathIndex.ValueRW.value = (pathIndex.ValueRO.value + 1) % path.Length;
            }
            transform.Position += math.normalize(direction) * SystemAPI.Time.DeltaTime * speed.ValueRO.value;
        }
    }
}