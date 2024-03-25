using Unity.Entities;
using Unity.Physics;
using UnityEngine;

public class CannonAuthoring : MonoBehaviour
{
    [Tooltip("The projectile prefab the tower will shoot")]
    public GameObject Projectile;
    [Tooltip("The time between each shot of the tower")]
    public float FireRate;
    [Tooltip("The maximum detection range of the tower")]
    public float Range;
    [Tooltip("The layer used to detect targets")]
    public LayerMask TargetingLayer;
    [Tooltip("The transform point from which the projectile will be shot")]
    public Transform ProjectileSpawnPoint;

    public class Baker : Baker<CannonAuthoring>
    {
        public override void Bake(CannonAuthoring authoring)
        {
            Entity bakingEntity = GetEntity(TransformUsageFlags.Renderable); // I need the tower to be visible in the game view
            
            // Builds the collision filter that will be used to detect enemies
            var filter = CollisionFilter.Default;
            filter.BelongsTo = (uint)authoring.Projectile.layer; // We consider that we should be able to collide with the target as being a projectile
            filter.CollidesWith = (uint)authoring.TargetingLayer.value; // We use this layer to define what can be targeted by the tower
   
            // For the non dynamic data we use a BlobAsset
            // A BlobAsset is a serialized read only data structure that can hold configuration data that does not change at runtime
            // Once create we can store a reference to that blob asset instead of carrying around all the data
            // This as the benefit of reducing the memory footprint of our entity
            // a more details explanation is available in this video : https://youtu.be/eYpjrQFqFkg?si=zTe0oydWj5buO7my
            
            BlobAssetReference<CannonConfig> bar;
            
            // To create a blob asset we need to allocate a BlobBuilder 
            using (var bb = new BlobBuilder(Unity.Collections.Allocator.Temp))
            {
                // Then we build the data structure for our configuration data
                ref CannonConfig tc = ref bb.ConstructRoot<CannonConfig>();
                tc.FiringRate = authoring.FireRate;
                tc.Range = authoring.Range;
                tc.Filter = filter;
                tc.Offset = authoring.ProjectileSpawnPoint.transform.position - authoring.transform.position;
                
                // and we get the reference of the blob asset
                bar = bb.CreateBlobAssetReference<CannonConfig>(Unity.Collections.Allocator.Persistent);
            }
            
            // Once we have the blob asset reference we can add it to the BlobAssetStore using this line
            AddBlobAsset(ref bar, out var hash); // This makes sure we don't create a duplicate blobasset in case another baker built the exact same with the exact same data
            
            // If you can precompute or use a guid as hash you can add it to the BlobAssetStore
            /*
                if(!TryGetBlobAssetReference(hash, out bar))
                {
                    // Create the blob asset
                    
                    // Add the blob asset
                    AddBlobAssetWithCustomHash(ref bar,hash);
                }
            */
            // Finally we can add the BlobAssetReference to a component of our entity
            AddComponent(bakingEntity,new TowerConfigBlobAsset()
            {
                Config = bar
            });
            
            // We setup a component to hold the "dynamic" data for the tower
            AddComponent(bakingEntity,new CannonData()
            {
                TimeToNextShoot = authoring.FireRate,
                ProjectilePrefab = GetEntity(authoring.Projectile,TransformUsageFlags.Dynamic)
            });
        } 
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
