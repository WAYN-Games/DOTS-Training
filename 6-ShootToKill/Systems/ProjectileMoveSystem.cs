
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

// If this is not clear, refer to 3-Spawning -> SpawnerSystem
[UpdateAfter(typeof(TransformSystemGroup))]
[BurstCompile]
public partial struct ProjectileMoveSystem : ISystem
{
    // We'll need a world transform lookup to find the position of the target in case the projectile is guided 
    private ComponentLookup<LocalToWorld> _positions;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _positions = SystemAPI.GetComponentLookup<LocalToWorld>(true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _positions.Update(ref state);
        
        // Schedule a parallel job that uses aspect to move the projectiles
        state.Dependency = new ProjectileMoveJob()
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            Positions = _positions
        }.ScheduleParallel(state.Dependency);
    }

    public partial struct ProjectileMoveJob : IJobEntity
    {
        public float DeltaTime;
        [ReadOnly] public ComponentLookup<LocalToWorld> Positions;

        private void Execute(ProjectileAspect projectile)
        {
            projectile.Move(DeltaTime,Positions);
        }
    }
}