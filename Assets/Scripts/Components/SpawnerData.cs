using Unity.Entities;

public struct SpawnerData : IComponentData
{
    public Entity Prefab;
    public float Timer;
    public float TimeToNextSpawn;
}
