using UnityEngine;

public class Jelly_Fish_Jump : MonoBehaviour
{
    public float bounceForce = 10f; // Base force applied when bouncing
    public float maxJumpHeight = 5f; // Maximum height the player can reach
    private Rigidbody playerRigidbody;

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object colliding is the player
        if (collision.gameObject.CompareTag("Player"))
        {
            playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();

            if (playerRigidbody != null)
            {
                // Calculate the current vertical velocity
                float currentVerticalVelocity = playerRigidbody.linearVelocity.y;

                // Ensure the player doesn't exceed the max jump height
                if (playerRigidbody.transform.position.y < maxJumpHeight)
                {
                    // Apply bounce force
                    playerRigidbody.linearVelocity = new Vector3(
                        playerRigidbody.linearVelocity.x,
                        Mathf.Max(bounceForce, currentVerticalVelocity),
                        playerRigidbody.linearVelocity.z
                    );
                }
            }
        }
    }
}