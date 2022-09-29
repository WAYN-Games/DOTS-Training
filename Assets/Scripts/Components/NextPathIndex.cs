using System;
using Unity.Entities;

[Serializable]
public struct NextPathIndex : IComponentData
{
    public int value;
}