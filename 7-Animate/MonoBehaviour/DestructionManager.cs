using UnityEngine;
using UnityEngine.Events;

public class DestructionManager : MonoBehaviour
{
    [Tooltip("Time to wait to actually destroy the game object. this should be sufficient to let all events play through.")]
    public float Delay;
    [Tooltip("The list of event to trigger before destroying the game object.")]
    public UnityEvent Events;

    public void Destroy()
    {
        Events.Invoke();
        Destroy(gameObject,Delay);
    }
}
