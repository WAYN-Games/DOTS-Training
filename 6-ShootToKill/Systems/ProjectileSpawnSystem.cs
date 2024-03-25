using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
[BurstCompile]
public partial struct ProjectileSpawnSystem : ISystem
{
    
    ComponentLookup<LocalToWorld> _positionLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PhysicsWorldSingleton>();
        state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
        _positionLookup = SystemAPI.GetComponentLookup<LocalToWorld>(true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecbBis = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        
        _positionLookup.Update(ref state);
        
        foreach (var (towerData, towerConfig, transform) 
                 in SystemAPI.Query<RefRW<CannonData>, RefRO<TowerConfigBlobAsset>, RefRO<LocalToWorld>>())
        {
            
            towerData.ValueRW.TimeToNextShoot -= SystemAPI.Time.DeltaTime;
            
            if (!(towerData.ValueRO.TimeToNextShoot < 0)) continue;
            
            
            ref CannonConfig tc = ref towerConfig.ValueRO.Config.Value;
                
            // Here we use a specific collector to only get the closest entity to the tower
            var closestHitCollector = new ClosestHitCollector<DistanceHit>(tc.Range);
            if (!physicsWorld.OverlapSphereCustom(transform.ValueRO.Position, tc.Range, ref closestHitCollector, tc.Filter)) continue;
                
                
            towerData.ValueRW.TimeToNextShoot = tc.FiringRate;
            
            
            Entity e = ecbBis.Instantiate(towerData.ValueRO.ProjectilePrefab);
            float3 position = transform.ValueRO.Position + towerConfig.ValueRO.Config.Value.Offset;
            quaternion rotation = TransformHelpers.LookAtRotation(position,
                _positionLookup[closestHitCollector.ClosestHit.Entity].Position,transform.ValueRO.Up);
              
            ecbBis.SetComponent(e, LocalTransform.FromPositionRotation(position,rotation));
            ecbBis.AddComponent(e, new Target() { Value = closestHitCollector.ClosestHit.Entity });
        }
    }
}
