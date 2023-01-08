using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct Waypoints : IBufferElementData
{
    public float3 value;   
}

public struct BlobPath
{
    public BlobArray<float3> waypoints;
}

public struct PathAsset : IComponentData
{
    public BlobAssetReference<BlobPath> path;
}