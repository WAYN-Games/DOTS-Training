using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct ClearInputSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var input in SystemAPI.Query<DynamicBuffer<TowerPlacementInput>>())
        {
            // Clear the input to avoid placing another tower next frame 
            input.Clear();
        }
    }
}