using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// This component supports all the dynamic data used by a defense tower.
/// </summary>
public struct CannonData : IComponentData
{
    /// <summary>
    /// Time remaining until the tower fires another projectile.
    /// </summary>
    public float TimeToNextShoot;
    /// <summary>
    /// The entity reference of the projectile prefab to shoot from the tower.
    /// We can use an entity reference in the context of an IComponentData because it does support entity remapping.
    /// </summary>
    public Entity ProjectilePrefab;
}