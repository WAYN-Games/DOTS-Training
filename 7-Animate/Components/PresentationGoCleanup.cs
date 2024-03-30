using Unity.Entities;
using UnityEngine;

/// <summary>
/// A managed cleanup component allowing to store an instance of game object that is used for rendering and other non ecs compatible features.
/// </summary>
public class PresentationGoCleanup : ICleanupComponentData
{
    public GameObject Instance;
}