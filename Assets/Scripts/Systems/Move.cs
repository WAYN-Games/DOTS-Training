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
        float deltaTime = Time.DeltaTime;
        
        Entities.WithAll<RenderMesh>().ForEach((ref Translation translation,ref NextPathIndex pathIndex,
            in DynamicBuffer<Waypoints> path, in Rotation rotation, in Speed speed) => {
                float3 direction = path[pathIndex.value].value - translation.Value;
                if(math.distance(translation.Value,path[pathIndex.value].value) < 0.1f)
                {
                    pathIndex.value = (pathIndex.value + 1) % path.Length;
                }
                translation.Value += math.normalize(direction) * deltaTime* speed.value;
        }).Schedule();
    }
}
