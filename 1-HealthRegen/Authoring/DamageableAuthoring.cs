using Unity.Entities;
using UnityEngine;

/// <summary>
/// This make an entity damageable but also able to recover health over time.
/// </summary>
public class DamageableAuthoring : MonoBehaviour
{

    [Tooltip("The total health the entity has.")]
    public float MaxHealth;
    [Tooltip("The amount of health the entity will regain every second")]
    public float HealthRegenPerSec;
    
    /// <summary>
    /// This is the class in charge of baking the data of the DamageableAuthoring component into IComponentData for the baked entity.
    /// Note : The baking process takes place in a dedicated world, that is not the default world.
    /// </summary>
    public class Baker : Baker<DamageableAuthoring>
    {
        public override void Bake(DamageableAuthoring authoring)
        {
            
            // This will get a reference to the entity created for the GameObject being baked.
            // Unlike MonoBehaviour which always have a transform, Entities can be created with no transform information. 
            // Here the TransformUsageFlags.None indicate that for this behaviour, the baked entity does not need to any transform information.
            // It will still have transform information due to other bakers. 
            //  TransformUsageFlags.None => Nothing (This is the default for empty game object)
            //  TransformUsageFlags.Renderable => Adds LocalToWorld (This is automatically set if you have a Mesh Renderer on your game object)
            //  TransformUsageFlags.Dynamic => Adds LocalToWorld, LocalTransform (This is automatically set if you have a Collider on your game object or if you the game object rereferenced is a Prafab) 
            //  TransformUsageFlags.WorldSpace => Adds LocalToWorld Remove Parent (This is automatically force if the game object is set as static in the inspector)
            //  TransformUsageFlags.NonUniformScale => Adds LocalToWorld, PostTransformMatrix  (This is automatically set if you game object has a non uniform scale
            //  TransformUsageFlags.ManualOverride => N/A

            Entity bakingEntity = GetEntity(TransformUsageFlags.None);

            // We create the Health component and populate it with the authoring data
            Health health = default;
            health.Max = authoring.MaxHealth;
            // Even though we only have one health information from the authoring component we still can populate several IComponentData fields.
            health.Current = authoring.MaxHealth / 2f;
     
            // Then we add the component to the baked entity
            AddComponent(bakingEntity,health);
            
            // We do the same thing for the HealthRegen component
            // As before, you see that we can add several IComponentData to the entity even if we only have one authoring MonoBehaviour
            AddComponent(bakingEntity,new HealthRegen()
            {
                PointPerSec = authoring.HealthRegenPerSec
            });
        }
    }
}
