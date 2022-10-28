using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public partial struct SpawnerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<BeginFixedStepSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach(var (spawner,path) in SystemAPI.Query<RefRW<SpawnerData>, DynamicBuffer<Waypoints>>())
        {
            spawner.ValueRW.TimeToNextSpawn -= SystemAPI.Time.DeltaTime;
            if(spawner.ValueRO.TimeToNextSpawn < 0)
            {
                spawner.ValueRW.TimeToNextSpawn = spawner.ValueRO.Timer;
                Entity e = ecb.Instantiate(spawner.ValueRO.Prefab);
                var buffer = ecb.AddBuffer<Waypoints>(e);
                buffer.AddRange(path.AsNativeArray());
                ecb.AddComponent<NextPathIndex>(e);
            }
        }
    }
}
