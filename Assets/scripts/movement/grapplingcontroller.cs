using UnityEngine;

public class PlayerGrapplingController : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public float grappleFov = 100f;

    [HideInInspector]
    public Vector3 pendingVelocity = Vector3.zero;

    public void ApplyPendingVelocity(Vector3 velocity)
    {
        pendingVelocity = velocity;

        if (cam != null)
            cam.fieldOfView = grappleFov;
    }

    public void ResetFov()
    {
        if (cam != null)
            cam.fieldOfView = 85f;
    }

    public Vector3 CalculateJumpVelocity(Vector3 start, Vector3 end, float arcHeight)
    {
        Vector3 toTarget = end - start;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0f, toTarget.z);
        float xzDistance = toTargetXZ.magnitude;
        float yOffset = toTarget.y;

        float gravity = Mathf.Abs(Physics.gravity.y);
        arcHeight = Mathf.Max(arcHeight, 0.1f);

        float timeToApex = Mathf.Sqrt(2 * arcHeight / gravity);
        float timeFromApex = Mathf.Sqrt(2 * Mathf.Max(arcHeight - yOffset, 0.1f) / gravity);
        float totalTime = timeToApex + timeFromApex;

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(2 * gravity * arcHeight);
        Vector3 velocityXZ = toTargetXZ / totalTime;

        return velocityXZ + velocityY;
    }
}
