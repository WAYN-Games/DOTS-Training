using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

public partial class Move : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = World.Time.DeltaTime;
        
        Entities.WithAll<RenderBounds>().ForEach((
            ref TransformAspect transform,
            ref NextPathIndex pathIndex,
            in DynamicBuffer<Waypoints> path, 
            in Speed speed) => {
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
public partial struct MoveISystem : ISystem
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
        foreach (var pathFollower 
            in SystemAPI.Query<
                PathFollowerAspect>()
                .WithAll<RenderBounds>())
        {
            pathFollower.FollowPath(SystemAPI.Time.DeltaTime);
      }
    }
}