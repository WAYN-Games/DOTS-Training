using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    public float Speed;
    public List<Transform> Waypoints;
}

[TemporaryBakingType]//[BakingType]
public struct TagComponent : IComponentData
{

}

public class EnemyBaker : Baker<EnemyAuthoring>
{
    public override void Bake(EnemyAuthoring authoring)
    {
        Speed speed = default;
        speed.value = authoring.Speed;  
        AddComponent(speed);

        AddComponent<TagComponent>();

        if (authoring.Waypoints == null || authoring.Waypoints.Count == 0) return;

        AddComponent<NextPathIndex>();
        DynamicBuffer<Waypoints> path = AddBuffer<Waypoints>();

        foreach(var point in authoring.Waypoints)
        {

            Waypoints wp = default;
            wp.value = point.position;
            path.Add(wp);
        }
        Debug.Log("Baker is called");
    }
}

[WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
public partial class EnemyBakingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithAll<TagComponent>().ForEach((in DynamicBuffer<Waypoints> path) => {
            Debug.Log($"This entity has {path.Length} waypoints.");
        }).Run();
    }
}