using Unity.Entities;

/// <summary>
/// This component store the information used by the WaveSpawnerSystem to spawn enemies at regular interval
/// </summary>
public struct SpawnerData : IComponentData
{
    /// <summary>
    /// This is a reference to the enemy prefab entity that will be spawned
    /// </summary>
    public Entity EnemyPrefab;
    /// <summary>
    /// The time that need to elapse before another enemy is spawned.
    /// </summary>
    public float SpawnInterval;
    /// <summary>
    /// The time remaining until the next enemy is spawned.
    /// </summary>
    public float TimeToNextSpawn;
}
