using Unity.Entities;
using UnityEngine;

public class ProjectileAuthoring : MonoBehaviour
{
    public float Speed;
    public GameObject ImpactVFX;
    public int NbHits;

    class Baker : Baker<ProjectileAuthoring>
    {
        public override void Bake(ProjectileAuthoring authoring)
        {
            AddComponent(new Speed() { value = authoring.Speed });
            if(authoring.NbHits > 1)
            {
                AddBuffer<HitList>();
            }
            AddComponent(new Impact() { Prefab = GetEntity(authoring.ImpactVFX), MaxImpactCount = authoring.NbHits });
        }
    }
}

public struct Impact : IComponentData
{
    public Entity Prefab;
    public int MaxImpactCount;
}
public struct HitList : IBufferElementData {
    public Entity Entity;
}