using Unity.Mathematics;
using Unity.Physics;

/// <summary>
/// This struct supports all the read only configuration of a tower.
/// You will notice that we are not referencing the prefab projectile entity in this struct.
/// This is because the struct will be used as part of blob asset that do not support entity remapping.
/// </summary>
public struct CannonConfig
{
    /// <summary>
    /// The time required between each shoot of the tower.
    /// </summary>
    public float FiringRate;
    /// <summary>
    /// The collision filter used by the tower to detect enemies.
    /// </summary>
    public CollisionFilter Filter;
    /// <summary>
    /// The detection and firing range of the tower.
    /// </summary>
    public float Range;    
    /// <summary>
    /// An offset from the tower position from where the projectile will be fired.
    /// </summary>
    public float3 Offset;
}