using UnityEngine;

public class PickupObject : MonoBehaviour
{
    public string pickupMessage = "Picked up the object!";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(pickupMessage);

            // Optionally: Add to inventory here

            Destroy(gameObject); // Removes the pickup from the scene
        }
    }
}
