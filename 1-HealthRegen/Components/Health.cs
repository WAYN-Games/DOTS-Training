using Unity.Entities;

/// <summary>
/// A IComponentData is a data unit structure that can be associated to an entity and queried by systems to operate on.
/// This data structure can only contain blittable data which essentially means primitive types and other unmanaged structs.
///  More info here : https://docs.unity3d.com/Packages/com.unity.entities@1.0/manual/components-unmanaged.html
/// </summary>
public struct Health : IComponentData
{
    public float Current;
    public float Max;
}