using Unity.Entities;
using UnityEngine;

/// <summary>
/// This allows to enable the debug animation system.
/// Entities that have this component will log the animation they re currently playing. 
/// </summary>
public class AnimatorCompanionLoggingAuthoring : MonoBehaviour
{   
    public class AnimatorCompanionLoggingBaker : Baker<AnimatorCompanionLoggingAuthoring>
    {
        public override void Bake(AnimatorCompanionLoggingAuthoring authoring)
        {
            Entity bakingEntity = GetEntity(TransformUsageFlags.None);
            AddComponent<EnabledAnimatorLogging>(bakingEntity);
        }
    }
}