using Unity.Entities;

public struct LimitedLifeTime : IComponentData
{
    public float TimeRemaining;
}