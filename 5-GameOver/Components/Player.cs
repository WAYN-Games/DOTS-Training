using Unity.Entities;

/// <summary>
/// This struct supports all the read only configuration of a tower.
/// You will notice that we are not referencing the prefab projectile entity in this struct.
/// This is because the struct will be used as part of blob asset that do not support entity remapping.
/// </summary>
public struct Player : IComponentData
{
    /// <summary>
    /// The number of enemy that can reach the village before game over;
    /// </summary>
    public int LifeCount;
}