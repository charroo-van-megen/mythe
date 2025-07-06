using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("LV1 1 (TEXTURES)");

            PlayerGroundTracker tracker = other.GetComponent<PlayerGroundTracker>();
            if (tracker != null)
            {
                other.transform.position = tracker.lastSafePosition;
            }
        }
    }
}
