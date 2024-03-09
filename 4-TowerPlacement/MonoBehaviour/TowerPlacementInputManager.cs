using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This class handles the player inputs.
/// </summary>
public class TowerPlacementInputManager : MonoBehaviour
{
    [Tooltip("InputAction to acquire the mouse position on the screen when clicked")]
    public InputAction Input;
    
    [Tooltip("The index of the tower in the tower store to spawn")]
    public int TowerIndex;
    
    [Tooltip("The physics layer used by the ray cast to find hte world position")]
    public LayerMask BelongsTo;
    [Tooltip("The physics layer the tower can be placed on")]
    public LayerMask CollidesWith;
    

    /// <summary>
    /// The main camera used to compute teh world position from the screen position. 
    /// </summary>
    private Camera _camera;
    
    /// <summary>
    /// The cached entity that carries the input data to t he ECS world
    /// </summary>
    private Entity _entity;
    
    /// <summary>
    /// The cached ECS World
    /// </summary>
    private World _world;

    private CollisionFilter _filter;
    
    private void OnEnable()
    {
        // Setup input management
        Input.performed += MouseClicked;
        Input.Enable();

        // Cache the main camera of the scene
        _camera = Camera.main;

        // Cache the ECS world reference
        _world = World.DefaultGameObjectInjectionWorld;
        
        // Initialize the collision filter that will be used by the physics system to find the position in the world where the tower will be spawned 
        _filter = CollisionFilter.Default;
        _filter.BelongsTo = (uint)BelongsTo.value;
        _filter.CollidesWith = (uint)CollidesWith.value;
    }

    private void MouseClicked(InputAction.CallbackContext ctx)
    {
        // Check if we already have the entity to cary our inputs
        if(_world.IsCreated && !_world.EntityManager.Exists(_entity))
        {
            // If not we create it
            _entity = _world.EntityManager.CreateEntity();
            _world.EntityManager.AddBuffer<TowerPlacementInput>(_entity);
        }
        
        // We get the position of the mouse
        var screenPosition = ctx.ReadValue<Vector2>();
        
        // convert it to a ray coming from the camera
        UnityEngine.Ray ray = _camera.ScreenPointToRay(screenPosition);

        // populate the input that wil be consumed by the ECS systems
        var towerPlacementInput = new TowerPlacementInput()
        {
            Value = new RaycastInput()
            {
                Start = ray.origin,
                Filter = _filter,
                End = ray.GetPoint(_camera.farClipPlane)
            },
            StoreIndex = TowerIndex
        };
        
        // Add the input to the buffer
        var inputBuffer = _world.EntityManager.GetBuffer<TowerPlacementInput>(_entity);
        inputBuffer.Add(towerPlacementInput);

    }

    private void OnDisable()
    {
        
        Input.started -= MouseClicked;
        Input.Disable();

        // Clean up our input entity
        if (_world.IsCreated && _world.EntityManager.Exists(_entity))
        {
            _world.EntityManager.DestroyEntity(_entity);
        }

    }
}