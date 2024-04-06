using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [Tooltip("The UI Slider that is used to represent the health")]
    [SerializeField] private Slider HpBar;
    
    /// <summary>
    /// Cached main camera
    /// </summary>
    private Camera _camera;
    
    void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
       
        // billboard make the ui face the camera  
        HpBar.transform.rotation = _camera.transform.rotation;
    }

    public void SetHealth(Health health)
    {
        HpBar.value = health.Current / health.Max;
    }
}
