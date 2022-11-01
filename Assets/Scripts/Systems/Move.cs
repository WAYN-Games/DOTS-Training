using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

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
        var ecbBOS = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (pathFollower,entity)
            in SystemAPI.Query<
                PathFollowerAspect>().WithEntityAccess())
        {
            pathFollower.FollowPath(SystemAPI.Time.DeltaTime);
            if(pathFollower.HasReachedEndOfPath())
            {
                ecbBOS.DestroyEntity(entity);
            }
      }
    }
}