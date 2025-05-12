using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    public UnityEvent uEvent;
    public GameObject TriggerObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == TriggerObject)
        {
            uEvent.Invoke();
        }
    }
}
