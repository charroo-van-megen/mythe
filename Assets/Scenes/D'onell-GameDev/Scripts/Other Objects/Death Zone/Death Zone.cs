using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerGroundTracker tracker = other.GetComponent<PlayerGroundTracker>();
            if (tracker != null)
            {
                other.transform.position = tracker.lastSafePosition;
            }
        }
    }
}
