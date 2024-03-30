using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Jobs;

/// <summary>
/// This system will spawn a game object for an entity
/// It will create a bidirectional link between the two
/// It will kee pa reference to all the monobehaviour of the game object in managed component on the entity
/// </summary>
[UpdateInGroup(typeof(InitializationSystemGroup))]
[RequireMatchingQueriesForUpdate]
public partial struct PresentationGoInitialisationSystem : ISystem
{
    
    // We can't use burst in this system because we are iterating over managed components
    public void OnUpdate(ref SystemState state)
    {
        // AddComponentObject is not available for a single entity in Entity Command Buffer
        // We need to use the query builder
        foreach (Entity entity in SystemAPI.QueryBuilder()
                     .WithAll<LocalToWorld,PresentationGo>()
                     .WithNone<PresentationGoCleanup>()
                     .Build().ToEntityArray(Allocator.Temp))
        {
            var presentationGo = SystemAPI.ManagedAPI.GetComponent<PresentationGo>(entity);
            
            // We create a new instance of the game object from the prefab stored in the PresentationGo
            GameObject go = Object.Instantiate(presentationGo.Prefab); 
            
            // Adding a reference to all the MonoBehaviour of the game object to the entity 
            var components = go.GetComponents(typeof(Component));
            foreach(Component cpt in components)
            {
                if (cpt != null)
                {
                    state.EntityManager.AddComponentObject(entity, cpt);
                }
                    
            }
            
            // Adding a bidirectional link between entity and game object
            go.AddComponent<EntityGameObject>().AssignEntity(entity, state.World);
            state.EntityManager.AddComponentData(entity, new PresentationGoCleanup() { Instance = go});
                        
            var localToWorld = SystemAPI.GetComponent<LocalToWorld>(entity);
            // Set the game object position to be identical to the entity position
            go.transform.position = localToWorld.Position;
            go.transform.rotation = localToWorld.Rotation;
        }
        
    }
}