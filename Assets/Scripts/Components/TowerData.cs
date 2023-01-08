using Unity.Entities;
using Unity.Physics;

public struct TowerData : IComponentData
{
    public float TimeToNextSpawn;       //4
    public Entity Prefab;               //8
}

public struct TowerConfig : IComponentData
{
    public float Timer;                 //4
    public CollisionFilter Filter;      //12
    public float Range;
}

public struct TowerConfigAsset : IComponentData
{
    public BlobAssetReference<TowerConfig> Config;    
}
