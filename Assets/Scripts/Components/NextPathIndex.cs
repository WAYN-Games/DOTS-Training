using System;
using Unity.Entities;

[Serializable]
[GenerateAuthoringComponent]
public struct NextPathIndex : IComponentData
{
    public int value;
}