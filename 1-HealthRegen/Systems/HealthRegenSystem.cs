using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// This system will regenerate the health of an entity over time
/// </summary>
public partial struct HealthRegenSystem : ISystem//, ISystemStartStop
{
/*
    void OnCreate(ref SystemState state)
    {
        // Nothing for this system
    }
*/

/*
    public void OnStartRunning(ref SystemState state)
    {
        // mandatory if you have ISystemStartStop
        throw new System.NotImplementedException();
    }
*/

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // We use an idiomatic foreach statement to loop over all the entities that have both the components "Health" and "HealthRegen"
        // to find those entities we use the SystemAPI.Query and specify if we want to be able to read the component (RefRO) or to both read and write to it (RefRW)
        foreach (var (hpRW,hpRegenRO) in SystemAPI.Query<RefRW<Health>,RefRO<HealthRegen>>())
        {
            // We then add the regen amount multiplied by the delta time to the current health of the entity
            // notice that what we get from the SystemAPI.Query is not the component itself but a wrapped reference to it
            // that is why we need to call ValueRW to write or ValueRO to read from the component.
            float newHealth = hpRW.ValueRO.Current + hpRegenRO.ValueRO.PointPerSec * SystemAPI.Time.DeltaTime;
            hpRW.ValueRW.Current = math.min(newHealth, hpRW.ValueRO.Max);
            Debug.DrawLine(Vector3.zero, Vector3.one*3,Color.blue,5);
        }
    }
    
/*
    public void OnStopRunning(ref SystemState state)
    {
        // mandatory if you have ISystemStartStop
        throw new System.NotImplementedException();
    }
*/

/*
    void OnDestroy(ref SystemState state)
    {
        // Nothing for this system
    }
*/




}