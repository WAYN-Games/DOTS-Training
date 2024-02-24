using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PathFollowAuthoring : MonoBehaviour
{
    public SpeedScriptableObject MovementSpeed;
    /// <summary>
    /// The set of path points the enemy will have to follow after spawning.
    /// Here we used a link statement to find the transform of all child GameObject of the spawner.
    /// </summary>
    public List<Transform> PathToFollow = new();
    
    public class EnemyBaker : Baker<PathFollowAuthoring>
    {
        public override void Bake(PathFollowAuthoring authoring)
        {
            
            // This will get a reference to the entity created for the GameObject being baked.
            // We specify TransformUsageFlags.Dynamic because we need the enemies to have both
            // "Unity.Transforms.LocalToWorld" (for it to be rendered in the world) and
            // "Unity.Transforms.LocalTransform" so we can move the entity in the world
            var bakingEntity = GetEntity(TransformUsageFlags.Dynamic);

            
            if (authoring.PathToFollow.Count > 0)
            {
                // Since IComponentData can store list type information we can use a different type of component.
                // A dynamic buffer allows us to store list type information on an entity.
                // A IBufferElementData describe a single element of a dynamic buffer.
                DynamicBuffer<Waypoints> pathBuffer = AddBuffer<Waypoints>(bakingEntity);
                foreach (Transform waypoint in authoring.PathToFollow)
                {
                    pathBuffer.Add(new Waypoints() { Value = waypoint.position });
                }
            }
            

            Speed speed = default;
            speed.value = authoring.MovementSpeed.Speed;
            AddComponent(bakingEntity,speed);

            // Tells the baking workflow that a change to this asset should trigger a re-bake of this entity.
            DependsOn(authoring.MovementSpeed);
            
            NextPathIndex nextPathIndex = default;
            nextPathIndex.value = 0;
            AddComponent(bakingEntity,nextPathIndex);
            
            Debug.Log($"Baking PathFollower");
        }
    }
}