using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct Waypoints : IBufferElementData
{
    public float3 value;   
}
