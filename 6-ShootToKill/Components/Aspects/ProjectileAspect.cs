using Unity.Entities;
using Unity.Transforms;

/// <summary>
/// An aspect is a wrapper for a set of component that together provide certain behavior.
/// It can be used to organise code making it reusable across systems and jobs.
/// It also allows to circumvent some limitations related to the maximum number of component is queries.
/// </summary>
public readonly partial struct ProjectileAspect : IAspect
{
    // An aspect can reference 
    //   A single Entity field to store the entity's ID
    //   RefRW<T> and RefRO<T> fields to access component data of type T, where T implements IComponentData.
    //   EnabledRefRW and EnabledRefRO fields to access the enabled state of components that implement IEnableableComponent.
    //   DynamicBuffer<T> fields to access the buffer elements that implement IBufferElementData
    //   Any ISharedComponent fields to access the shared component value as read-only.
    //   Other aspect types
    // Note that aspect don't work with tag components
    // Data access can be declared Read Only (RefRO) or Read/Write (RefRW)
    // Nested aspect and dynamic buffer are by default Read/Write, to  declare them read only, use the [ReadOnly] Attribute
    // You can also use the [Optional] attribute to declare a non mandatory data
    
    readonly RefRO<Guided> _guided;
    readonly RefRO<Speed> _speed;
    readonly RefRO<Target> _target;
    readonly RefRW<LocalTransform> _transform;

    // Then you can declare a method using the component to perform a behavior
    public void Move(float time, ComponentLookup<LocalToWorld> positions)
    {
        if (_guided.ValueRO.Enbaled && positions.HasComponent(_target.ValueRO.Value))
        {
            _transform.ValueRW.Rotation = TransformHelpers.LookAtRotation(_transform.ValueRW.Position,
                positions[_target.ValueRO.Value].Position, _transform.ValueRW.Up());
        }

        _transform.ValueRW.Position += _speed.ValueRO.value * time * _transform.ValueRW.Forward();
    }

}

