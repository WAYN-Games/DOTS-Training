using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct PathFollowerAspect : IAspect
{
    [Optional]
    readonly RefRO<Speed> speed;
    readonly RefRW<NextPathIndex> pathIndex;
    readonly RefRO<PathAsset> pathAsset;
    readonly TransformAspect transform;

    public void FollowPath(float time)
    {
        ref var path = ref pathAsset.ValueRO.path.Value.waypoints;
        float3 direction = path[pathIndex.ValueRO.value] - transform.WorldPosition;
        if (math.distance(transform.WorldPosition, path[pathIndex.ValueRO.value]) < 0.1f)
        {
            pathIndex.ValueRW.value = (pathIndex.ValueRO.value + 1) % path.Length;
        }
        float movementSpeed = speed.IsValid ? speed.ValueRO.value : 1;
        transform.LocalPosition += math.normalize(direction) * time * movementSpeed;
        transform.LookAt(path[pathIndex.ValueRO.value]);
    }

    public bool HasReachedEndOfPath()
    {
        ref var path = ref pathAsset.ValueRO.path.Value.waypoints;
        return math.distance(transform.WorldPosition, path[path.Length - 1]) < 0.1f;
    }

}