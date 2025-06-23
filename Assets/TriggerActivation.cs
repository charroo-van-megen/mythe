using UnityEngine;

public class TriggerActivation : MonoBehaviour
{
    public GameObject EventTrigger; // Assign the other trigger GameObject in the Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the object entering is tagged as "Player"
        {
            Debug.Log("TriggerActivation: Player entered the first trigger.");

            if (EventTrigger != null)
            {
                // Enable the target trigger's collider
                Collider targetCollider = EventTrigger.GetComponent<Collider>();
                if (targetCollider != null)
                {
                    targetCollider.enabled = true;
                    Debug.Log("TriggerActivation: Second trigger (BridgeCollapse) enabled.");
                }
                else
                {
                    Debug.LogWarning("TriggerActivation: No collider found on the assigned EventTrigger object.");
                }
            }
            else
            {
                Debug.LogWarning("TriggerActivation: EventTrigger not assigned in the Inspector.");
            }
        }
    }
}
