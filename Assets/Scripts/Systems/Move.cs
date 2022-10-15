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
