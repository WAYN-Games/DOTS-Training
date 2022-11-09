using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    public float Speed;
    public GameObject Prefab;
}

public class PresentationGO : IComponentData
{
    public GameObject Prefab;
}
public class TransformGO : ICleanupComponentData
{
    public Transform Transform;
}

public class AnimatorGO : IComponentData
{
    public Animator Animator;
}


public class EnemyBaker : Baker<EnemyAuthoring>
{
    public override void Bake(EnemyAuthoring authoring)
    {
        if (authoring.Speed > 0)
        {
            Speed speed = default;
            speed.value = authoring.Speed;
            AddComponent(speed);
        }

        PresentationGO pgo = new PresentationGO();
        pgo.Prefab = authoring.Prefab;
        AddComponentObject(pgo);
    }
}
