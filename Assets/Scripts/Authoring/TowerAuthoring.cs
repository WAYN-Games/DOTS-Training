using Unity.Collections.LowLevel.Unsafe;
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
                TimeToNextSpawn = authoring.FireRate,
                Prefab = GetEntity(authoring.Projectile)
        });

            BlobAssetReference<TowerConfig> bar;
            using (var bb = new BlobBuilder(Unity.Collections.Allocator.Temp))
            {
                ref TowerConfig tc = ref bb.ConstructRoot<TowerConfig>();
                tc.Timer = authoring.FireRate;
                tc.Range = authoring.Range;
                tc.Filter = filter;
                bar = bb.CreateBlobAssetReference<TowerConfig>(Unity.Collections.Allocator.Persistent);
            }
            AddBlobAsset(ref bar, out var hash);
            AddComponent(new TowerConfigAsset()
            {
                Config = bar
            });
        } 
    }
}
