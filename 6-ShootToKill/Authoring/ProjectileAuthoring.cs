using Unity.Entities;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;

public class ProjectileAuthoring : MonoBehaviour
{
    [Tooltip("The prefab that will be spawn upon impact of the projectile")]
    public GameObject ProjectileImpactVfx;
    [Tooltip("The movement speed of the projectile")]
    public float ProjectileSpeed;
    [Tooltip("The maximum number of target the projectile can hit")]
    public int NbHits = 1;
    [Tooltip("Does the projectile follow it's target (true) or moves in a straight line (false) ?")]
    public bool IsGuided = false;
    
    public class Baker : Baker<ProjectileAuthoring>
    {
        public override void Bake(ProjectileAuthoring authoring)
        {
            
            Entity bakingEntity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(bakingEntity, new Speed()
            {
                value = authoring.ProjectileSpeed
            });

            AddComponent(bakingEntity, new Projectile()
            {
                MaxImpactCount = authoring.NbHits
            });
            
            AddBuffer<HitList>(bakingEntity);
            AddComponent(bakingEntity, new Guided(){Enbaled =  authoring.IsGuided});

            if (authoring.ProjectileImpactVfx == null) return;
            AddComponent(bakingEntity, new Impact()
            {
                Prefab = GetEntity(authoring.ProjectileImpactVfx, TransformUsageFlags.Renderable)
            });
        }
    }
}