using UnityEngine;

public class Jelly_Fish_Jump : MonoBehaviour
{
    public float launchForce = 15f;
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            New_Movement playerMovement = other.GetComponent<New_Movement>();
            if (playerMovement != null && playerMovement.velocity.y <= 0f)
            {
                playerMovement.velocity.y = launchForce;
            }
        }
    }
}
