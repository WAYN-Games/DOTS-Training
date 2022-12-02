using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
[BurstCompile]
public partial struct InputSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        DynamicBuffer<Towers> towers = SystemAPI.GetSingletonBuffer<Towers>();
        var ecbBOS = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var input in SystemAPI.Query<DynamicBuffer<TowerPlacementInput>>())
        {
            foreach(var placementInput in input)
            {
                if(physicsWorld.CastRay(placementInput.Value,out var hit))
                {
                    Debug.Log($"{hit.Position}");
                    var towerPosition = math.round(hit.Position) + math.up();
                    NativeList<DistanceHit> distances = new NativeList<DistanceHit>(Allocator.Temp);
                    if (!physicsWorld.OverlapSphere(towerPosition + math.up(), 0.1f, ref distances, CollisionFilter.Default))
                    {
                        Entity e = ecbBOS.Instantiate(towers[placementInput.index].Prefab);
                        var transform = LocalTransform.Identity;
                        transform.Position = towerPosition;
                        ecbBOS.SetComponent(e, transform);
                    }
                }
            }
            input.Clear();
        }

    }
}