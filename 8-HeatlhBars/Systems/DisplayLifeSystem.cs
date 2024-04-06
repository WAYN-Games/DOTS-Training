using Unity.Entities;
using UnityEngine;

public partial struct DisplayPlayerLifeSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // Notice the use of WithChangeFilter here
        // it means that this loop will only be executed if another systems accessed the Player component in read/write
        // note that the change filter works based on the access and not the actual write
        // and it is scoped at the chunk level, not at teh entity level
        // if you want a detailed explanation this is a great resource : https://gametorrahod.com/designing-an-efficient-system-with-version-numbers/
        foreach (var player 
                 in SystemAPI.Query<RefRO<Player>>()
                     .WithChangeFilter<Player>())
        {
            if (PlayerUIController.Instance == null) return;
            Debug.Log($"Updating Player UI");
            PlayerUIController.Instance.SetHealth(player.ValueRO.LifeCount);
        }
    }
}

public partial struct DisplayEnemyLifeSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (health,hpBarController) 
                 in SystemAPI.Query<RefRO<Health>, SystemAPI.ManagedAPI.UnityEngineComponent<HealthBarController>>()
                     .WithChangeFilter<Health>())
        {
            hpBarController.Value.SetHealth(health.ValueRO);
            Debug.Log($"Updating Enemy UI");
        }
    }
}
