using Unity.Entities;
using UnityEngine;

/// <summary>
/// A managed component allowing to store a prefab game object that will be used for rendering and other non ecs compatible features.
/// </summary>
public class PresentationGo : IComponentData
{
    public GameObject Prefab;
}