using UnityEngine;

public class PlayerGroundTracker : MonoBehaviour
{
    public Vector3 lastSafePosition;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            lastSafePosition = transform.position;
        }
    }
}
