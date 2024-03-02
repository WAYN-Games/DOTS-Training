using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// This class exposes the configuration of the spawning behavior of the enemy waves.
/// </summary>
public class SpawnerAuthoring : MonoBehaviour
{

    [Tooltip("The prefab of the enemy that will be spawned")]
    public GameObject EnemyPrefab;
    
    [Tooltip("The time that need to elapse before another enemy is spawned.")]
    public float SpawnInterval;
    
    /// <summary>
    /// The set of path points the enemy will have to follow after spawning.
    /// Here we used a link statement to find the transform of all child GameObject of the spawner.
    /// </summary>
    List<Transform> Path => GetComponentsInChildren<Transform>().Where(go => go.gameObject != this.gameObject).ToList();

    /// <summary>
    /// This class bake the WaveSpawnerAuthoring supporting the configuration of the waves into an Entity with a set of components that can be processes by a ISystem or SystemBase
    /// </summary>
    public class SpawnerBaker : Baker<SpawnerAuthoring>
    {
        /// <summary>
        /// The baking method. 
        /// </summary>
        /// <param name="authoring">The MonoBehaviour to bake into an entity</param>
        public override void Bake(SpawnerAuthoring authoring)
        {
    
            // This will get a reference to the entity created for the GameObject being baked.
            // Unlike MonoBehaviour which always have a transform, Entities can be created with no transform information. 
            // Here the TransformUsageFlags.WorldSpace indicate that for this entity to work properly it will need the "Unity.Transforms.LocalToWorld" components.
            // The "Unity.Transforms.LocalToWorld" component is required for the spawner to have a world position, which will be used in WaveSpawnerSystem as the spawning position fo the enemy
            var bakingEntity = GetEntity(TransformUsageFlags.WorldSpace);
            
            // The SpawnerData is a struct IComponentData that will carry the simple data configuration. 
            // struct IComponentData can only hold blittable types (more information in this video : https://www.youtube.com/watch?v=uXvuFMlBI3Y&t=73s)
            SpawnerData sd = default;
            sd.SpawnInterval = authoring.SpawnInterval;
            sd.TimeToNextSpawn = authoring.SpawnInterval;

            // Here we get the prefab entity baked for the enemy prefab GameObject,
            // we specify TransformUsageFlags.Dynamic because we need the enemies to have both "Unity.Transforms.LocalToWorld" (for it to be rendered in the world)
            // and "Unity.Transforms.LocalTransform" s owe can move the entity in the world
            sd.EnemyPrefab = GetEntity(authoring.EnemyPrefab,TransformUsageFlags.Dynamic); 
            
            // Finally we add the component to the entity
            AddComponent(bakingEntity,sd);

            DynamicBuffer<Waypoints> buffer = AddBuffer<Waypoints>(bakingEntity);
            
            for (int i = 0; i < authoring.Path.Count; i++)
            {
                buffer.Add(new Waypoints(){Value = authoring.Path.ElementAt(i).position});
            }
            
        }
    }

}


