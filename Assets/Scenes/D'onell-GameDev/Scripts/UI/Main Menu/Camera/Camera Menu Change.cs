using UnityEngine;

public class CameraMenuChange : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float rotateSpeed = 2f;

    private Transform currentTarget;
    private bool isMoving = false;

    void Update()
    {
        if (isMoving && currentTarget != null)
        {
            // Smooth position
            transform.position = Vector3.Lerp(transform.position, currentTarget.position, Time.deltaTime * moveSpeed);

            // Smooth rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, currentTarget.rotation, Time.deltaTime * rotateSpeed);

            // Stop when close enough
            if (Vector3.Distance(transform.position, currentTarget.position) < 0.01f &&
                Quaternion.Angle(transform.rotation, currentTarget.rotation) < 0.5f)
            {
                isMoving = false;
            }
        }
    }

    // 🔘 Call this from your UI buttons and pass in the desired target
    public void MoveToTarget(Transform target)
    {
        currentTarget = target;
        isMoving = true;
    }
}