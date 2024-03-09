using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

// We update after the FixedStepSimulationSystemGroup
// which contains the physics update to use the latest PhysicsWorldSingleton available
[UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(ClearInputSystem))]
[BurstCompile]
public partial struct TowerPlacementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<PhysicsWorldSingleton>();
        state.RequireForUpdate<TowerPrefabs>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        DynamicBuffer<TowerPrefabs> towers = SystemAPI.GetSingletonBuffer<TowerPrefabs>();
        EntityCommandBuffer ecbBos = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        
        foreach (var input in SystemAPI.Query<DynamicBuffer<TowerPlacementInput>>())
        {
            foreach(TowerPlacementInput placementInput in input)
            {
                // Find the world position the tower should be placed at
                if (!physicsWorld.CastRay(placementInput.Value, out var hit)) continue;
                
                // If the position of the tower was found
                
                float3 towerPosition = hit.Position;
                    
                // Built a collision filter that collides with everything but the terrain
                var anythingButTerrain = CollisionFilter.Default;
                anythingButTerrain.CollidesWith = ~placementInput.Value.Filter.CollidesWith;
                    
                // Check if there is anything but terrain at teh future position of the new tower
                NativeList<DistanceHit> distances = new NativeList<DistanceHit>(Allocator.Temp);
                if (physicsWorld.OverlapSphere(towerPosition, 1f, ref distances,
                        anythingButTerrain))
                    continue; // If there is a tower, a path, a mountain or anything but terrain, we skip the creation of the tower

                Debug.Log("Tower palced");
                // instantiate the tower at the position
                Entity e = ecbBos.Instantiate(towers[placementInput.StoreIndex].Prefab);
                LocalTransform transform = LocalTransform.Identity;
                transform.Position = towerPosition;
                ecbBos.SetComponent(e, transform);
            }
            
        }

    }
}