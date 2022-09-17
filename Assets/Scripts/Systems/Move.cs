using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

public partial class Move : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        
        
        
        Entities.WithAll<RenderMesh>().ForEach((ref Translation translation, in Rotation rotation) => {
            translation.Value += math.mul(rotation.Value, new float3(0, 0, 1)) * deltaTime;
        }).Schedule();
    }
}
