using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public partial struct GameOverCleanupSystem : ISystem
{
    private EntityQuery _query;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _query = SystemAPI.QueryBuilder().WithAll<GameOver>().Build();
        state.RequireForUpdate(_query);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Debug.Log($"GameOver");
        state.EntityManager.DestroyEntity(_query.GetSingletonEntity());

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp,PlaybackPolicy.SinglePlayback);
        foreach (var (buffer,entity) in SystemAPI.Query<DynamicBuffer<Waypoints >>().WithEntityAccess())
        {
            ecb.AddComponent<Disabled>(entity);
        }
        ecb.Playback(state.EntityManager);
        // Not necessary because I'm using Allocator.Temp, otherwise you should dispose of the ECB
        //ecb.Dispose();
    }
}