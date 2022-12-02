using Unity.Entities;
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

        foreach(var (spawner,path,position) in SystemAPI.Query<RefRW<SpawnerData>, DynamicBuffer<Waypoints>,TransformAspect>())
        {
            spawner.ValueRW.TimeToNextSpawn -= SystemAPI.Time.DeltaTime;
            if(spawner.ValueRO.TimeToNextSpawn < 0)
            {
                spawner.ValueRW.TimeToNextSpawn = spawner.ValueRO.Timer;
                Entity e = ecb.Instantiate(spawner.ValueRO.Prefab);
                Debug.Log($"{position.WorldPosition}");
                var transformLocal = LocalTransform.Identity;
                transformLocal.Position = position.LocalPosition;
                ecb.SetComponent(e, transformLocal);

                var transformWorld = WorldTransform.Identity;
                transformWorld.Position = position.WorldPosition;
                ecb.SetComponent(e, transformWorld);
                var buffer = ecb.AddBuffer<Waypoints>(e);
                buffer.AddRange(path.AsNativeArray());
                ecb.AddComponent<NextPathIndex>(e);
            }
        }
    }
}
