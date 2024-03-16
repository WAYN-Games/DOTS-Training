using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    [Tooltip("The number of enemy that can reach the village before game over")]
    public int LifeCount;

    public class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity bakingEntity = GetEntity(TransformUsageFlags.None);
            AddComponent(bakingEntity,new Player()
            {
                LifeCount = authoring.LifeCount
            });
        } 
    }
}
