using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

public class TowerAuthoring : MonoBehaviour
{
    public GameObject Projectile;
    public float FireRate;
    public float Range;

    class Baker : Baker<TowerAuthoring>
    {
        public override void Bake(TowerAuthoring authoring)
        {
            var filter = CollisionFilter.Default;
            filter.CollidesWith = authoring.Projectile.GetComponent<PhysicsShapeAuthoring>().CollidesWith.Value;
            filter.BelongsTo = authoring.Projectile.GetComponent<PhysicsShapeAuthoring>().BelongsTo.Value;


            AddComponent(new TowerData()
            {
                Prefab = GetEntity(authoring.Projectile),
                Timer = authoring.FireRate,
                TimeToNextSpawn = authoring.FireRate,
                Range = authoring.Range,
                Filter = filter
            });
        }
    }
}
