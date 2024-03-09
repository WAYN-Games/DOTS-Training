using Unity.Entities;
using Unity.Physics;

public struct TowerPlacementInput : IBufferElementData
{
    /// <summary>
    /// The information about the ray cast to perform to figure out where to place a tower.
    /// </summary>
    public RaycastInput Value;
    
    /// <summary>
    /// The index of the tower in the tower store to spawn
    /// </summary>
    public int StoreIndex;
}