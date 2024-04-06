using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerUIController : MonoBehaviour
{
    #region SINGLETON_MONOBEHAVIOUR

        public static PlayerUIController Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
        }

    #endregion
    
    [Tooltip("The UI Document supporting the player's UI")]
    public UIDocument UIDocument;
    
    /// <summary>
    /// The query to fetch the singleton player data from ECS.
    /// </summary>
    private EntityQuery _query;


    private void Start()
    {
        UIDocument.rootVisualElement.Q<Button>(name:"Add").clicked += IncreaseHealth;
        UIDocument.rootVisualElement.Q<Button>(name:"Decrease").clicked += DecreaseHealth;
        _query = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(new ComponentType(typeof(Player)));
    }

    /// <summary>
    /// Reduce the health of the player by 1
    /// </summary>
    private void DecreaseHealth()
    {
        Player p = _query.GetSingleton<Player>();
        p.LifeCount -= 1;
        _query.SetSingleton(p);
    }

    /// <summary>
    /// Increase the health of the player by 1
    /// </summary>
    private void IncreaseHealth()
    {        
        RefRW<Player> p = _query.GetSingletonRW<Player>();
        p.ValueRW.LifeCount += 1;
    }

    /// <summary>
    /// Updates the player's Health slider UI
    /// </summary>
    /// <param name="percentage"></param>
    public void SetHealth(float percentage)
    {
        if (UIDocument == null) return;
        var hp = UIDocument.rootVisualElement.Q<ProgressBar>();
        hp.value = percentage;
    }
}


