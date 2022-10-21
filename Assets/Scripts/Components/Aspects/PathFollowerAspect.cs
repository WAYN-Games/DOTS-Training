using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct PathFollowerAspec : IAspect
{
    [Optional]
    readonly RefRO<Speed> speed;
    readonly RefRW<NextPathIndex> pathIndex;
    [ReadOnly]
    readonly DynamicBuffer<Waypoints> path;
    readonly TransformAspect transform;

    public void FollowPath(float time)
    {
        float3 direction = path[pathIndex.ValueRO.value].value - transform.Position;
        if (math.distance(transform.Position, path[pathIndex.ValueRO.value].value) < 0.1f)
        {
            pathIndex.ValueRW.value = (pathIndex.ValueRO.value + 1) % path.Length;
        }
        float movementSpeed = speed.IsValid ? speed.ValueRO.value : 1;
        transform.Position += math.normalize(direction) * time * movementSpeed;

    }
}