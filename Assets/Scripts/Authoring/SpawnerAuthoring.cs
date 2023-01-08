using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SpawnerAuthoring : MonoBehaviour
{

    public GameObject Prefab;
    public float Timer;
    public List<Transform> Path => GetComponentsInChildren<Transform>().Where(go => go.gameObject != this.gameObject).ToList();
}

public class SpawnerBaker : Baker<SpawnerAuthoring>
{
    public override void Bake(SpawnerAuthoring authoring)
    {
        /*DynamicBuffer<Waypoints> path = AddBuffer<Waypoints>();
        foreach(var point in authoring.Path)
        {
            Waypoints wp = default;
            wp.value = point.position;
            path.Add(wp);
        }*/
        BlobAssetReference<BlobPath> bar;
        using (var bb = new BlobBuilder(Allocator.Temp))
        {
            ref BlobPath blobPath = ref bb.ConstructRoot<BlobPath>();

            BlobBuilderArray<float3> waypoints = bb.Allocate(ref blobPath.waypoints, authoring.Path.Count);
            for (int i = 0; i < authoring.Path.Count; i++)
            {
                waypoints[i] = authoring.Path.ElementAt(i).position;
            }
            bar = bb.CreateBlobAssetReference<BlobPath>(Allocator.Persistent);
        }
        AddBlobAsset(ref bar, out var hash);
        AddComponent(new PathAsset() { path = bar });

        SpawnerData sd = default;
        sd.Prefab = GetEntity(authoring.Prefab);
        sd.Timer = authoring.Timer;
        sd.TimeToNextSpawn = authoring.Timer;
        AddComponent(sd);
    }
}
