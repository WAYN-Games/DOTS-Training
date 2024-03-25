using Unity.Entities;

public struct TowerConfigBlobAsset : IComponentData
{
    public BlobAssetReference<CannonConfig> Config;    
}