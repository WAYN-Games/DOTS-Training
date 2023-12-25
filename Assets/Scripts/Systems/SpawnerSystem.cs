using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(TransformSystemGroup))]
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
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach(var (spawner,path,position) in SystemAPI.Query<RefRW<SpawnerData>, RefRO<PathAsset>,RefRO<LocalTransform>>())
        {
            spawner.ValueRW.TimeToNextSpawn -= SystemAPI.Time.DeltaTime;
            if(spawner.ValueRO.TimeToNextSpawn < 0)
            {
                spawner.ValueRW.TimeToNextSpawn = spawner.ValueRO.Timer;
                Entity e = ecb.Instantiate(spawner.ValueRO.Prefab);
                var transformLocal = LocalTransform.Identity;
                transformLocal.Position = position.ValueRO.Position;
                ecb.SetComponent(e, transformLocal);

                               
                ecb.AddComponent<PathAsset>(e);
                ecb.SetComponent(e, path.ValueRO);
                ecb.AddComponent<NextPathIndex>(e);
            }
        }
    }
}
