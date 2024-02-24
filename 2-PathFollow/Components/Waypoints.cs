using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// A IBufferElementData is teh definition of a single element that can be store in list like structure call ed a dynamic buffer.
/// The InternalBufferCapacity attribute defines the number of element that will be store in the chunk
/// More information about chunk memory layout in this video https://www.youtube.com/watch?v=eYpjrQFqFkg&list=PL6ubahbodJ3N2udo4n9yGQcpbWnqdgYnL&index=14
/// </summary>
[InternalBufferCapacity(0)]
public struct Waypoints : IBufferElementData
{
    public float3 Value;   
}