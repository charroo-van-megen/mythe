using UnityEngine;

public class Fish : MonoBehaviour
{
    [Header("Target to swim around")]
    public Transform target; // The object the fish will swim around

    [Header("Swimming Settings")]
    public float radius = 3f;            // Distance from the target
    public float rotationSpeed = 20f;    // Speed of circling
    public float swimBobAmount = 0.5f;   // How much it bobs up/down
    public float swimBobSpeed = 2f;      // How fast it bobs

    private float angle = 0f;
    private Vector3 offset;

    void Update()
    {
        if (target == null) return;

        // Increase angle over time to move in a circle
        angle += rotationSpeed * Time.deltaTime;
        float radians = angle * Mathf.Deg2Rad;

        // Calculate new position around the target
        float x = Mathf.Cos(radians) * radius;
        float z = Mathf.Sin(radians) * radius;
        float y = Mathf.Sin(Time.time * swimBobSpeed) * swimBobAmount;

        Vector3 offsetPos = new Vector3(x, y, z);
        Vector3 newPos = target.position + offsetPos;

        // Move fish to new position
        transform.position = newPos;

        // Get the next point along the circle (for smooth facing)
        float nextAngle = angle + 1f;
        float nextX = Mathf.Cos(nextAngle * Mathf.Deg2Rad) * radius;
        float nextZ = Mathf.Sin(nextAngle * Mathf.Deg2Rad) * radius;
        Vector3 nextPos = target.position + new Vector3(nextX, y, nextZ);

        // Make the fish look where it's going (not toward the center)
        Vector3 direction = (nextPos - transform.position).normalized;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }
}
