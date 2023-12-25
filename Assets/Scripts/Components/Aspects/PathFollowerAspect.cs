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
    readonly RefRW<LocalTransform> transform;

    public void FollowPath(float time)
    {
        ref var path = ref pathAsset.ValueRO.path.Value.waypoints;
        float3 direction = path[pathIndex.ValueRO.value] - transform.ValueRO.Position;
        if (math.distance(transform.ValueRO.Position, path[pathIndex.ValueRO.value]) < 0.1f)
        {
            pathIndex.ValueRW.value = (pathIndex.ValueRO.value + 1) % path.Length;
        }
        float movementSpeed = speed.IsValid ? speed.ValueRO.value : 1;
        transform.ValueRW.Position += math.normalize(direction) * time * movementSpeed;
        transform.ValueRW.Rotation = TransformHelpers.LookAtRotation(transform.ValueRO.Position,path[pathIndex.ValueRO.value],transform.ValueRW.Up());
    }

    public bool HasReachedEndOfPath()
    {
        ref var path = ref pathAsset.ValueRO.path.Value.waypoints;
        return math.distance(transform.ValueRO.Position, path[path.Length - 1]) < 0.1f;
    }

}