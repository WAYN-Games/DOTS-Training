using Unity.Entities;
using Unity.Physics;

public struct TowerData : IComponentData
{
    public Entity Prefab;
    public float Timer;
    public float TimeToNextSpawn;
    public CollisionFilter Filter;
    public float Range;
}
