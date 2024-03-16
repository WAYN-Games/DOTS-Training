using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

// If this is not clear, refer to 4-TowerPlacement -> TowerPlacementSystem
[UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
[BurstCompile]
public partial struct EnemyPlayerCollisionSystem : ISystem
{
    // A Component or Buffer lookup is an indexing container that provide access to the data of an arbitrary entity.
    // It involves random memory access so it's less performant than other means of iterating through entities.
    BufferLookup<Waypoints> _enemyLookup;
    ComponentLookup<Player> _playerLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        
        // When we get the lookup we need to specify if we want to read only or read and write to the data
        _enemyLookup = SystemAPI.GetBufferLookup<Waypoints>(true);
        _playerLookup = SystemAPI.GetComponentLookup<Player>(false);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // We use the BeginInitializationEntityCommandBufferSystem to create entity, add component, set component data,
        // this ensures the new entity or data goes through any necessary initialization system and runs before the transform system group avoiding any flickering 
        EntityCommandBuffer ecbBis = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        // We use the BeginSimulationEntityCommandBufferSystem to destroy entity, remove component
        // this ensures that no command remains to be executed after an entity gets destroyed
        EntityCommandBuffer ecbBss = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        
        // The SimulationSingleton carries the result of the physics simulation
        SimulationSingleton simulation = SystemAPI.GetSingleton<SimulationSingleton>();

        // Lookups need to be updated before use, some data may have been moved, so the index need to be updated
        _enemyLookup.Update(ref state);
        _playerLookup.Update(ref state);
        
        // We schedule a job to process the physics trigger events on a worker thread
        // this let the main thread work on other entity/components in the mean time
        state.Dependency = new EnemyPlayerCollisionJob()
        {
            Enemies = _enemyLookup,
            Players = _playerLookup,
            EcsBss = ecbBss,
            EcsBis = ecbBis
        }.Schedule(simulation, state.Dependency);
    }

    
    [BurstCompile]
    private struct EnemyPlayerCollisionJob : ITriggerEventsJob
    {
        [ReadOnly] public BufferLookup<Waypoints> Enemies;
        public ComponentLookup<Player> Players;
        
        public EntityCommandBuffer EcsBis;
        public EntityCommandBuffer EcsBss;
        
        public void Execute(TriggerEvent triggerEvent)
        {
            // The event contains a pair of entity involved in the trigger
            // the first thing to do is to figure out which entity is which
            var (playerEntity,enemy) = IdentifyEntityPair(triggerEvent);

            // then we make sure the identified entities are part of the pair of entities that are handled by this job
            if (!ShouldProcess(playerEntity, enemy)) return;

            // Damage player
            Player player = Players[playerEntity];
            player.LifeCount -= 1;
            Players[playerEntity] = player;
            
            // Destroy the enemy entity 
            EcsBss.DestroyEntity(enemy);

            // If the player has lost all its lifes
            if (player.LifeCount > 0) return;
            // Declare the game over
            Entity gameOverEntity = EcsBis.CreateEntity();
            EcsBis.AddComponent<GameOver>(gameOverEntity);
        }

        private static bool ShouldProcess(Entity projectile, Entity enemy)
        {
            // If one of the entity is Entity.Null, it means we did not manage to identify a processable entity pair
            return !Entity.Null.Equals(projectile) && !Entity.Null.Equals(enemy);
        }

        private (Entity,Entity) IdentifyEntityPair(TriggerEvent triggerEvent)
        {           
            Entity player = Entity.Null;
            Entity enemy = Entity.Null;
            
            // For each entity involved in the even, we check if it is available in one of the lookup 
            if (Players.HasComponent(triggerEvent.EntityA))
                player = triggerEvent.EntityA;
            if (Players.HasComponent(triggerEvent.EntityB))
                player = triggerEvent.EntityB;
            if (Enemies.HasBuffer(triggerEvent.EntityA))
                enemy = triggerEvent.EntityA;
            if (Enemies.HasBuffer(triggerEvent.EntityB))
                enemy = triggerEvent.EntityB;
            return (player,enemy);
        }

    }
}