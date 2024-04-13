using Unity.Burst;
using Unity.Entities;

[UpdateBefore(typeof(GameOverCleanupSystem))]
[BurstCompile]
public partial struct ShowGameOverScreenSystem : ISystem
{
    private EntityQuery _query;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameOver>();
    }
    
    public void OnUpdate(ref SystemState state)
    {
        if(LevelManager.Instance !=null)
            LevelManager.Instance.ShowGameOverScreen();
    }
}