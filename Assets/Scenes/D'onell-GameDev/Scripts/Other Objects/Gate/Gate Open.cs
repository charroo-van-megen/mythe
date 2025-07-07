using UnityEngine;

public class GateOpen : MonoBehaviour
{
    public Rigidbody Gate;
    public float gateSpeed = 0.5f;
    public float targetY = -5f;

    void Start()
    {
        Gate = GetComponent<Rigidbody>();
        Gate.isKinematic = true; // Important to move manually
    }

    void Update()
    {
        if (!Locked.locked && Gate.position.y > targetY)
        {
            Vector3 newPosition = Gate.position + Vector3.down * gateSpeed * Time.deltaTime;

            // Clamp the Y position so it doesn’t go below the target
            if (newPosition.y < targetY)
                newPosition.y = targetY;

            Gate.MovePosition(newPosition);
        }
    }
}
