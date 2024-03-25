using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

// If this is not clear, refer to 4-TowerPlacement -> TowerPlacementSystem
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
[BurstCompile]
public partial struct ProjectileCollisionSystem : ISystem
{
    ComponentLookup<LocalTransform> _positionLookup;
    ComponentLookup<Impact> _impactLookup;
    ComponentLookup<Projectile> _projectileLookup;
    BufferLookup<HitList> _hitListLookup;
    ComponentLookup<Health> _healthLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        _positionLookup = SystemAPI.GetComponentLookup<LocalTransform>(true);
        _impactLookup = SystemAPI.GetComponentLookup<Impact>(false);
        _projectileLookup = SystemAPI.GetComponentLookup<Projectile>(true);
        _hitListLookup = SystemAPI.GetBufferLookup<HitList>();
        _healthLookup = SystemAPI.GetComponentLookup<Health>(false);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecbBis = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        EntityCommandBuffer ecbBss = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        SimulationSingleton simulation = SystemAPI.GetSingleton<SimulationSingleton>();

        _positionLookup.Update(ref state);
        _healthLookup.Update(ref state);
        _impactLookup.Update(ref state);
        _hitListLookup.Update(ref state);;
        _projectileLookup.Update(ref state);

        state.Dependency = new ProjectileHitJob()
        {
            Projectiles = _projectileLookup,
            Impacts = _impactLookup,
            EnemiesHealth = _healthLookup,
            Positions = _positionLookup,
            HitLists = _hitListLookup,
            EcbBis = ecbBis,
            EcsBss = ecbBss
        }.Schedule(simulation, state.Dependency);


    }

    [BurstCompile]
    private struct ProjectileHitJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<LocalTransform> Positions;
        [ReadOnly]public ComponentLookup<Projectile> Projectiles;
        public ComponentLookup<Health> EnemiesHealth;
        public ComponentLookup<Impact> Impacts;
        public BufferLookup<HitList> HitLists;
        public EntityCommandBuffer EcbBis;
        public EntityCommandBuffer EcsBss;

        public void Execute(TriggerEvent triggerEvent)
        {
            var (projectile,enemy) = IdentifyEntityPair(triggerEvent);

            if (!ShouldProcess(projectile, enemy)) return;

            // Check we did not already hit that target in previous frames
            if (TargetHasAlreadyBeenHit(projectile, enemy)) return;


            // Damage enemy
            Health hp = EnemiesHealth[enemy];
            hp.Current -= 5;
            EnemiesHealth[enemy] = hp;
            
            // Destroy projectile if it hits all its targets
            if (Projectiles[projectile].MaxImpactCount <= HitLists[projectile].Length)
                EcsBss.DestroyEntity(projectile);
            
            // Spawn VFX at impact point
            if (!Impacts.HasComponent(projectile)) return;
            Entity impactEntity = EcbBis.Instantiate(Impacts[projectile].Prefab);
            EcbBis.SetComponent(impactEntity, LocalTransform.FromPosition(Positions[enemy].Position));
        }

        private static bool ShouldProcess(Entity projectile, Entity enemy)
        {
            return !Entity.Null.Equals(projectile) && !Entity.Null.Equals(enemy);
        }

        private (Entity,Entity) IdentifyEntityPair(TriggerEvent triggerEvent)
        {           
            Entity projectile = Entity.Null;
            Entity enemy = Entity.Null;
            // Identify which entity is which
            if (Projectiles.HasComponent(triggerEvent.EntityA))
                projectile = triggerEvent.EntityA;
            if (Projectiles.HasComponent(triggerEvent.EntityB))
                projectile = triggerEvent.EntityB;
            if (EnemiesHealth.HasComponent(triggerEvent.EntityA))
                enemy = triggerEvent.EntityA;
            if (EnemiesHealth.HasComponent(triggerEvent.EntityB))
                enemy = triggerEvent.EntityB;
            return (projectile,enemy);
        }

        private bool TargetHasAlreadyBeenHit(Entity projectile, Entity enemy)
        {
            var hits = HitLists[projectile];
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].Entity.Equals(enemy))
                    return true;
            }

            // Add enemy to list of already hit entities
            // to avoid hitting it next frame due to the
            // stateless nature of the Physics
            hits.Add(new HitList { Entity = enemy });
            return false;
        }
    }
}